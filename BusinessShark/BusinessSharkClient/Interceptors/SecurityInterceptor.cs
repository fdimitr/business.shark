using Grpc.Core;
using Grpc.Core.Interceptors;

namespace BusinessSharkClient.Interceptors
{
    public class SecurityInterceptor : Interceptor
    {
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var newContext = PrepareInterceptorContext(context);
            return base.AsyncUnaryCall(request, newContext, continuation);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var newContext = PrepareInterceptorContext(context);
            return base.BlockingUnaryCall(request, newContext, continuation);
        }

        private static ClientInterceptorContext<TRequest, TResponse> PrepareInterceptorContext<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var headers = new Metadata
            {
                { "Authorization", $"Bearer { SecureStorage.Default.GetAsync("access_token").Result }" }
            };
            var newOptions = context.Options.WithHeaders(headers);
            var newContext = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, newOptions);
            return newContext;
        }
    }
}
