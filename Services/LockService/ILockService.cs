namespace LearningServer01
{
    public interface ILockService
    {
        Task<IAsyncDisposable> LockPlayerAsync(string id);
    }
}
