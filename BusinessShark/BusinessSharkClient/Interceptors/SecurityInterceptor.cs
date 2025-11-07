using BusinessSharkClient.Logic.System;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace BusinessSharkClient.Interceptors
{
    public class SecurityInterceptor : Interceptor
    {
        private readonly IAuthService _authService;
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        public SecurityInterceptor(IAuthService authService)
        {
            _authService = authService;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var newContext = PrepareInterceptorContextAsync(context).GetAwaiter().GetResult();
            var call = base.AsyncUnaryCall(request, newContext, continuation);

            return HandleAuthErrorsAsync(call, request, newContext, continuation);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            // gRPC sync вызовы
            var newContext = PrepareInterceptorContextAsync(context).GetAwaiter().GetResult();
            try
            {
                return base.BlockingUnaryCall(request, newContext, continuation);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                // Если токен истёк, пробуем обновить
                if (_authService.RefreshTokenAsync().Result)
                {
                    var retryContext = PrepareInterceptorContextAsync(context).GetAwaiter().GetResult();
                    return base.BlockingUnaryCall(request, retryContext, continuation);
                }

                throw;
            }
        }

        /// <summary>
        /// Проверяет токен, при необходимости делает refresh и подставляет актуальный Authorization header.
        /// </summary>
        private async Task<ClientInterceptorContext<TRequest, TResponse>> PrepareInterceptorContextAsync<TRequest, TResponse>(
            ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var token = await _authService.GetValidAccessTokenAsync();

            var headers = new Metadata
            {
                { "Authorization", $"Bearer {token}" }
            };

            var newOptions = context.Options.WithHeaders(headers);
            var newContext = new ClientInterceptorContext<TRequest, TResponse>(
                context.Method,
                context.Host,
                newOptions
            );

            return newContext;
        }

        /// <summary>
        /// Обрабатывает ошибку Unauthenticated (401), делает refresh токена и повторяет запрос.
        /// </summary>
        private AsyncUnaryCall<TResponse> HandleAuthErrorsAsync<TRequest, TResponse>(
            AsyncUnaryCall<TResponse> originalCall,
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            async Task<TResponse> ResponseHandler()
            {
                try
                {
                    return await originalCall.ResponseAsync;
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
                {
                    // Если токен недействителен, пробуем обновить
                    await _refreshLock.WaitAsync();
                    try
                    {
                        if (await _authService.RefreshTokenAsync())
                        {
                            var retryContext = await PrepareInterceptorContextAsync(context);
                            var retryCall = base.AsyncUnaryCall(request, retryContext, continuation);
                            return await retryCall.ResponseAsync;
                        }
                    }
                    finally
                    {
                        _refreshLock.Release();
                    }

                    throw;
                }
            }

            return new AsyncUnaryCall<TResponse>(
                ResponseHandler(),
                originalCall.ResponseHeadersAsync,
                originalCall.GetStatus,
                originalCall.GetTrailers,
                originalCall.Dispose);
        }
    }
}
