using GameDB;

namespace LearningServer01
{
    public interface ITableService
    {
        TableMetadata Metadata { get; }
        GameDBContainer Container { get; }
    }
}
