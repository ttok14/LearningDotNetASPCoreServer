namespace LearningServer01
{
    public class GameSettings
    {
        // appsettings.json의 "GameSettings" 섹션 하위 항목들과 이름이 일치해야 합니다.
        public string AppVersion { get; set; }
        public bool IsMaintenance { get; set; }
        public string CdnBaseUrl { get; set; }
    }
}
