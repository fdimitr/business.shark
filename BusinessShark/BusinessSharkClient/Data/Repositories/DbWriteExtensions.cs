using System.Diagnostics;

namespace BusinessSharkClient.Data.Repositories
{
    public static class DbWriteExtensions
    {
        public static async Task WriteAsync(Func<Task> action, CancellationToken token)
        {
            var tid = Thread.CurrentThread.ManagedThreadId;
            var taskId = Task.CurrentId;

            await WriteLock.GlobalWriteLock.WaitAsync(token);
            try
            {
                await action();
            }
            finally
            {
                WriteLock.GlobalWriteLock.Release();
            }
        }
    }
}
