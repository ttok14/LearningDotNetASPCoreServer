using JNetwork;
using LearningServer01.Config;
using LearningServer01.Repositories;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearningServer01.Services.AuthService
{
    public class AuthService : IAuthService
    {
        JwtSettings _jwtSettings;
        IPlayerRepository _repo;

        public AuthService(IOptions<JwtSettings> options, IPlayerRepository repo)
        {
            _jwtSettings = options.Value;
            _repo = repo;
        }

        string IAuthService.CreateToken(string userId)
        {
            // 토큰에 담을 정보 (Claim)
            // '누구인지' 에 대한 정보만 담아야함. 언제든지 이거는 중간에 볼수있음
            // (비번넣으면안됨)
            var claims = new List<Claim>()
            {
                /// jwt 페이로드에 포함됨 참고.
                /// 참고로 <see cref="ClaimTypes.NameIdentifier"/> 는 "nameid" 로 치환돼 들어감
                new Claim(ClaimTypes.NameIdentifier, userId)
                // 필요하다면 new Claim(ClaimTypes.Role, "Admin") 이런것도 참고 ㄱㄱ 
            };

            // 비밀키 가져오기
            var keyStr = _jwtSettings.SecretKey;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));

            // 서명자격증명
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            /// 토큰의 구성요소들 의미 참고 ㄱㄱ
            /// xxx.xxx.xxx 형태
            /// (header,payload,hash) 라고함 => hash 가 중요한데,
            /// 이걸 만들려면 서버 SecretKey 를 알아야 만들수있기때문에 진위여부 판정 가능
            ///     https://datatracker.ietf.org/doc/html/rfc7519#page-9 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        async Task<(ERROR_CODE errCode, string token, PlayerInfo? loggedInPlayerInfo)> IAuthService.LoginAsync(string id, string userPassword)
        {
            var player = await _repo.GetPlayerFullAsync(id, isReadonly: true);

            if (player == null)
                return (ERROR_CODE.FAIL_INVALID_USER, string.Empty, null);

            var @this = (IAuthService)this;
            if ((@this).VerifyPassword(userPassword, player.Password) == false)
                return (ERROR_CODE.LOGIN_PW_WRONG, string.Empty, null);

            var token = @this.CreateToken(id);

            return (ERROR_CODE.SUCCESS, token, player);
        }

        bool IAuthService.VerifyPassword(string rawPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(rawPassword, hashedPassword);
        }
    }
}
