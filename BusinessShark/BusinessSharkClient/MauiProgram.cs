using BusinessSharkClient.Interceptors;
using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.System;
using CommunityToolkit.Maui;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;

namespace BusinessSharkClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            string baseAddress = "http://10.0.2.2:5042";

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                // Initialize the .NET MAUI Community Toolkit by adding the below line of code
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .UseLiveCharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Lato-Bold.ttf", "latobold");
                    fonts.AddFont("Lato-Regular.ttf", "latoregular");
                    fonts.AddFont("Font-Awesome-Solid.ttf", "awesome-solid");
                });

            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
            var channel = GrpcChannel.ForAddress(baseAddress, new GrpcChannelOptions
            {
                HttpHandler = handler
            });

            var authServiceClient = new BusinessSharkService.AuthService.AuthServiceClient(channel);
            builder.Services.AddScoped(services => authServiceClient);
            builder.Services.AddScoped<IAuthService>(service => new AuthClientService(authServiceClient));
            builder.Services.AddSingleton<SecurityInterceptor>();

            builder.Services.AddSingleton(services =>
            {
                // Важно: инжектируем interceptor с DI
                var interceptor = services.GetRequiredService<SecurityInterceptor>();
                var invoker = channel.Intercept(interceptor);

                return invoker;
            });

            // Grpc clients
            builder.Services.AddScoped(services => 
            {
                var invoker = services.GetRequiredService<CallInvoker>();
                return new BusinessSharkService.Greeter.GreeterClient(invoker);
            });

            builder.Services.AddScoped(services =>
            {
                var invoker = services.GetRequiredService<CallInvoker>();
                return new BusinessSharkService.ProductDefinitionService.ProductDefinitionServiceClient(invoker);
            });

            builder.Services.AddScoped(services =>
            {
                var invoker = services.GetRequiredService<CallInvoker>();
                return new BusinessSharkService.ProductCategoryService.ProductCategoryServiceClient(invoker);
            });

            builder.Services.AddScoped(services =>
            {
                var invoker = services.GetRequiredService<CallInvoker>();
                return new BusinessSharkService.SummaryService.SummaryServiceClient(invoker);
            });

            builder.Services.AddScoped(services =>
            {
                var invoker = services.GetRequiredService<CallInvoker>();
                return new BusinessSharkService.CompanyService.CompanyServiceClient(invoker);
            });

            builder.Services.AddScoped(services =>
            {
                var invoker = services.GetRequiredService<CallInvoker>();
                return new BusinessSharkService.SawmillService.SawmillServiceClient(invoker);
            });

            builder.Services.AddScoped(services =>
            {
                var invoker = services.GetRequiredService<CallInvoker>();
                return new BusinessSharkService.DivisionTransactionsService.DivisionTransactionsServiceClient(invoker);
            });

            // Providers
            builder.Services.AddSingleton<GlobalDataProvider>();
            builder.Services.AddScoped<SawmillProvider>();
            builder.Services.AddScoped<DivisionTransactionProvider>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
