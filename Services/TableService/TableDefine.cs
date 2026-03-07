namespace LearningServer01
{
    public class TableFileInfo
    {
        public string Name { get; set; }
        public string Hash { get; set; }
        public long ByteSize { get; set; }
    }

    public class TableMetadata
    {
        public string Version { get; set; }
        public string TotalHash { get; set; }
        public List<TableFileInfo> Files { get; set; } = new List<TableFileInfo>();
    }
}
