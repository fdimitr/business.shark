namespace BusinessSharkClient.Data.Repositories
{
    public static class DbWriteExtensions
    {
        public static async Task WriteAsync(Func<Task> action, CancellationToken token)
        {
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
