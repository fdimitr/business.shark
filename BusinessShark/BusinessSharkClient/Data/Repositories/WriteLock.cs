namespace BusinessSharkClient.Data.Repositories
{
    public static class WriteLock
    {
        // Разрешает только 1 одновременную запись в SQLite
        public static readonly SemaphoreSlim GlobalWriteLock = new(1, 1);
    }
}
