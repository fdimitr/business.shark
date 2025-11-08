using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace BusinessSharkService.GrpcServices.Interceptors
{
    public class LoggingInterceptor : Interceptor
    {
        private readonly ILogger<LoggingInterceptor> _logger;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var stopwatch = Stopwatch.StartNew();
            var method = context.Method;
            var peer = context.Peer;

            _logger.LogDebug("gRPC call started: {Method} from {Peer}", method, peer);

            try
            {
                var response = await continuation(request, context);
                stopwatch.Stop();

                _logger.LogDebug("gRPC call finished: {Method} in {Elapsed} ms",
                    method, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (RpcException ex)
            {
                stopwatch.Stop();
                _logger.LogWarning(ex, "gRPC error in {Method}: {StatusCode}", method, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Unhandled error in gRPC method {Method}", method);
                throw;
            }
        }
    }
}
