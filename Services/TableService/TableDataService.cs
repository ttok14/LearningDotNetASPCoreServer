using GameDB;
using LearningServer01.Common;
using Newtonsoft.Json;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using System.Linq;
using System.Reflection;

namespace LearningServer01.Services.TableService
{
    public class TableDataService : ITableService
    {
        public const string BinaryExtension = "bytes";

        readonly FieldInfo[] ContainerFieldsCache = typeof(GameDBContainer).GetFields();

        private readonly IWebHostEnvironment _env;

        public TableMetadata Metadata { get; private set; }
        public GameDBContainer Container { get; set; } = new GameDBContainer();

        public TableDataService(IWebHostEnvironment env)
        {
            _env = env;

            Initialize();
        }

        void Initialize()
        {
            Console.WriteLine($"■■■■ 테이블 로드 시작 ■■■■");

            string metadataPath = Path.Combine(_env.ContentRootPath, "Table/table_metadata.json");

            if (File.Exists(metadataPath))
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine($"테이블 메타데이터가 발견 | 경로 : {metadataPath}");
                Console.ResetColor();
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"테이블 메타데이터가 없습니다 | 경로 : {metadataPath}");
                Console.ResetColor();
                return;
            }

            var json = File.ReadAllText(metadataPath);
            Metadata = JsonConvert.DeserializeObject<TableMetadata>(json);
            if (Metadata != null)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine($"테이블 메타데이터 파싱 성공");
                Console.WriteLine($"테이블 TotalHash : {Metadata.TotalHash}");
                Console.ResetColor();
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"테이블 메타데이터 파싱 실패 | Json : {json}");
                Console.ResetColor();
                return;
            }

            PreConfig();

            LoadTables();

            Console.WriteLine($"■■■■ 테이블 로드 끝 ■■■■");
        }

        void PreConfig()
        {
            RegisterMessagePackResolvers();
        }

        void LoadTables()
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine($"테이블 개수 : {Metadata.Files.Count}");
            Console.ResetColor();

            var validCheckHashes = Metadata.Files.Select(t => t.Hash).ToList();

            int loadedCount = 0;
            foreach (var field in ContainerFieldsCache)
            {
                var deserialized = LoadTableBinaryReadingFile(field, validator: (bytes) =>
                {
                    string hash = Helper.GetHash(bytes);
                    int idx = validCheckHashes.FindIndex(t => t == hash);

                    if (idx != -1)
                        validCheckHashes.RemoveAt(idx);

                    return idx != -1;
                });

                if (deserialized == null)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine($"테이블 로드 실패 : {field.Name}");
                    Console.ResetColor();
                    return;
                }

                field.SetValue(Container, deserialized);
                loadedCount++;

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"테이블 로딩 | 필드명: {field.Name}");
                Console.ResetColor();
            }

            if (validCheckHashes.Count > 0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"테이블 유효 검증할 대상이 남아있습니다. 에러");
                Console.ResetColor();
                return;
            }

            if (loadedCount != Metadata.Files.Count)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"테이블 개수 미스매칭 | 메타데이터 테이블 개수 : {Metadata.Files.Count} , 로드된 테이블 개수 : {loadedCount}");
                Console.ResetColor();
            }
        }

        public void RegisterMessagePackResolvers()
        {
            DBCompositeResolver.Instance.Register(
                GameDB.Resolvers.GameDBContainerResolver.Instance,
                MessagePack.Unity.UnityResolver.Instance,
                MessagePack.Resolvers.StandardResolver.Instance
            );

            var options = MessagePack.MessagePackSerializerOptions.Standard.WithResolver(DBCompositeResolver.Instance);
            MessagePack.MessagePackSerializer.DefaultOptions = options;
        }

        public object LoadTableBinaryReadingFile(FieldInfo fieldInfo, Predicate<byte[]> validator = null)
        {
            var tableType = fieldInfo.FieldType.GetGenericArguments()[1];
            return LoadTableBinaryReadingFile(tableType, Path.Combine(_env.ContentRootPath, $"Table/binaries/{tableType.Name}.{BinaryExtension}"), validator);
        }

        public object LoadTableBinaryReadingFile(Type tableType, string binPath, Predicate<byte[]> validator = null)
        {
            var bytesRead = File.ReadAllBytes(binPath);
            if (validator != null && validator.Invoke(bytesRead) == false)
            {
                return null;
            }

            return InvokeDeserialize(tableType, bytesRead);
        }

        private object InvokeDeserialize(Type tableType, byte[] bytes)
        {
            return tableType.InvokeMember("Deserialize", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { bytes });
        }
    }
}
