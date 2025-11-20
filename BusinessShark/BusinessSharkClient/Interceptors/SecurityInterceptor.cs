using BusinessSharkClient.Logic.System;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace BusinessSharkClient.Interceptors
{
    public class SecurityInterceptor(IAuthService authService) : Interceptor
    {
        // Лок для refresh token оставим, но лучше использовать тот, что внутри сервиса.
        // Здесь он нужен только если логика повтора сложная.

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            // МЫ НЕ ВЫЗЫВАЕМ .Result или .Wait() ЗДЕСЬ!

            // Создаем "ленивую" задачу, которая сделает всё: получит токен, вызовет сервис, обработает ошибку
            var responseCallTask = ExecuteAsyncCall(request, context, continuation);

            // Формируем обертку, которая делегирует ожидание нашей внутренней задаче
            var responseAsync = responseCallTask.ContinueWith(t => t.Result.ResponseAsync).Unwrap();
            var responseHeadersAsync = responseCallTask.ContinueWith(t => t.Result.ResponseHeadersAsync).Unwrap();

            // Функции для получения статуса и трейлеров (нужно дождаться создания реального вызова)
            Func<Status> getStatus = () => responseCallTask.Result.GetStatus();
            Func<Metadata> getTrailers = () => responseCallTask.Result.GetTrailers();
            Action dispose = () => responseCallTask.Result.Dispose();

            return new AsyncUnaryCall<TResponse>(
                responseAsync,
                responseHeadersAsync,
                getStatus,
                getTrailers,
                dispose
            );
        }

        private async Task<AsyncUnaryCall<TResponse>> ExecuteAsyncCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            // 1. Асинхронно получаем контекст с токеном (теперь безопасно!)
            var newContext = await PrepareInterceptorContextAsync(context).ConfigureAwait(false);

            // 2. Делаем реальный вызов
            var call = continuation(request, newContext);

            // 3. Запускаем "прослушивание" ответа, чтобы поймать 401 ошибку
            return new AsyncUnaryCall<TResponse>(
                HandleResponseAsync(call, request, context, continuation),
                call.ResponseHeadersAsync,
                call.GetStatus,
                call.GetTrailers,
                call.Dispose
            );
        }

        private async Task<TResponse> HandleResponseAsync<TRequest, TResponse>(
            AsyncUnaryCall<TResponse> currentCall,
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            try
            {
                // Пытаемся получить ответ
                return await currentCall.ResponseAsync.ConfigureAwait(false);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                // Токен протух. Пробуем обновить.
                var refreshed = await authService.RefreshTokenAsync().ConfigureAwait(false);
                if (refreshed)
                {
                    // Если обновили, создаем новый контекст и повторяем вызов
                    var retryContext = await PrepareInterceptorContextAsync(context).ConfigureAwait(false);
                    var retryCall = continuation(request, retryContext);
                    return await retryCall.ResponseAsync.ConfigureAwait(false);
                }
                throw;
            }
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            // ХАК ДЛЯ СИНХРОННЫХ ВЫЗОВОВ В MAUI
            // Task.Run переносит выполнение в ThreadPool, где нет UI SynchronizationContext.
            // Это предотвращает Deadlock при вызове .Result
            return Task.Run(async () =>
            {
                var newContext = await PrepareInterceptorContextAsync(context).ConfigureAwait(false);
                try
                {
                    return continuation(request, newContext);
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
                {
                    if (await authService.RefreshTokenAsync().ConfigureAwait(false))
                    {
                        var retryContext = await PrepareInterceptorContextAsync(context).ConfigureAwait(false);
                        return continuation(request, retryContext);
                    }
                    throw;
                }
            }).GetAwaiter().GetResult();
        }

        private async Task<ClientInterceptorContext<TRequest, TResponse>> PrepareInterceptorContextAsync<TRequest, TResponse>(
            ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            // Здесь обязательно ConfigureAwait(false)
            var token = await authService.GetValidAccessTokenAsync().ConfigureAwait(false);

            var headers = new Metadata
            {
                { "Authorization", $"Bearer {token}" }
            };

            var newOptions = context.Options.WithHeaders(headers);
            return new ClientInterceptorContext<TRequest, TResponse>(
                context.Method,
                context.Host,
                newOptions
            );
        }
    }
}