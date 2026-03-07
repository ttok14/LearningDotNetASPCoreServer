
using System.Collections.Concurrent;

namespace LearningServer01
{
    public class LockService : ILockService
    {
        // Remove 는 굳이 안해도 대갯지? 재접 타이밍/락 순서 등 신경쓸게많음...
        ConcurrentDictionary<string, SemaphoreSlim> _playerLocks = new();

        public async Task<IAsyncDisposable> LockPlayerAsync(string id)
        {
            var @lock = _playerLocks.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
            await @lock.WaitAsync();
            return new Releaser(@lock);
        }

        public class Releaser : IAsyncDisposable
        {
            public readonly SemaphoreSlim _sem;

            public Releaser(SemaphoreSlim semaphore)
            {
                _sem = semaphore;
            }

            public ValueTask DisposeAsync()
            {
                _sem.Release();
                return ValueTask.CompletedTask;
            }
        }
    }
}
