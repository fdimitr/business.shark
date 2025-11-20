using System.Threading.Channels;
using BusinessSharkClient.Data.Sync.Interfaces;

namespace BusinessSharkClient.Data.Sync
{
    public class SyncEngine(IEnumerable<ISyncHandler> handlers)
    {
        private readonly Channel<ISyncHandler> _queue = Channel.CreateUnbounded<ISyncHandler>();
        private readonly SemaphoreSlim _parallelism = new(Environment.ProcessorCount > 2 ? 2 : 1);

        public async Task StartCriticalSync()
        {
            // load and sorting handlers
            foreach (var handler in handlers.Where(h => h.Priority == SyncPriority.Critical))
            {
                await handler.PushAsync(CancellationToken.None);
                await handler.PullAsync(CancellationToken.None);
            }
        }

        public async Task StartBackgroundPush(CancellationToken token, ISyncHandler handler)
        {
            await Task.Run(() => handler.PushAsync(token), token);
        }

        public async Task StartBackgroundSync(CancellationToken token)
        {
            // load and sorting handlers
            foreach (var handler in handlers.Where(h=>h.Priority > SyncPriority.Critical).OrderBy(h => h.Priority))
                await _queue.Writer.WriteAsync(handler, token);

            // loading sync worker tasks
            for (int i = 0; i < 2; i++)
                _ = Task.Run(() => WorkerLoop(token), token);
        }

        private async Task WorkerLoop(CancellationToken token)
        {
            while (await _queue.Reader.WaitToReadAsync(token))
            {
                var handler = await _queue.Reader.ReadAsync(token);

                await _parallelism.WaitAsync(token);
                try
                {
                    await handler.PushAsync(token);
                    await handler.PullAsync(token);
                }
                finally
                {
                    _parallelism.Release();
                }
            }
        }
    }

}
