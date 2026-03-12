
using LearningServer01.Data;
using LearningServer01.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using LearningServer01.Services.TableService;
using LearningServer01.Services.PlayerService;
using LearningServer01.Config;
using LearningServer01.Services.AuthService;
using Serilog;
using LearningServer01.MemoryCache;
using LearningServer01.BackgroundServices;

namespace LearningServer01
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
                .WriteTo.File("logs/server-log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information($">>> 서버 시작 중 ...");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext());

                builder.Services.AddControllers(options =>
                {
                    options.Filters.Add<LogActionFilter>();
                }).AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

                // builder.Services.AddSingleton<IPlayerRepository, InMemoryPlayerRepository>();
                builder.Services.AddScoped<IPlayerRepository, DbPlayerRepository>();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                #region ====:: 커스텀 싱글턴 & Option 등록 ::====
                builder.Services.Configure<GameSettings>(builder.Configuration.GetSection("GameSettings"));
                builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

                builder.Services.AddHostedService<GlobalZombieBattleSessionCleaner>();

                builder.Services.AddSingleton<ILockService, LockService>();
                builder.Services.AddSingleton<ITableService, TableDataService>();

                builder.Services.AddSingleton<ActiveBattleCache>();

                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<IPlayerService, PlayerService>();
                #endregion

                #region ====:: DB 인증 설정 ::====
                string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    Log.Fatal($"DefaultConnection 을 가져올 수 없습니다. appsettings.json 확인할것");
                }
                else
                {
                    builder.Services.AddDbContext<AppDbContext>(options =>
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
                }
                #endregion

                #region ====:: JWT 인증 서비스 등록 ::====
                var jwtSettings = builder.Configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"];
                var key = Encoding.UTF8.GetBytes(secretKey);

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });
                #endregion

                var app = builder.Build();

                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    try
                    {
                        var tableService = services.GetRequiredService<ITableService>();
                        Log.Information($"테이블 데이터 로드 성공");
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex, $"치명적인 오류 발생 | 테이블 서비스 에러");
                        throw;
                    }
                }

                /// 미들웨어 설정 시작 

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseSerilogRequestLogging();

                // 아래는 호출 순서가 중요. 호출 순으로 처리

                // Authentication 이 Authorization 보다 앞에 위치해야함
                // Authentication : 너 누구냐 ? 
                app.UseAuthentication();
                // Authorization : 그래서 너 여기 들어갈 권한 있어? 
                app.UseAuthorization();

                // 올 통과면 컨트롤러로 연결
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"서버가 예기치 않게 중단되었습니다.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
