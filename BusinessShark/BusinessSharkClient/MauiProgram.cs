using BusinessSharkClient.Logic;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.Extensions.Logging;
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
                .ConfigureSyncfusionToolkit()
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
            var invoker = channel.Intercept(new Interceptors.SecurityInterceptor());

            builder.Services.AddScoped(services => new BusinessSharkService.Greeter.GreeterClient(invoker));
            builder.Services.AddScoped(services => new BusinessSharkService.AuthService.AuthServiceClient(channel));
            builder.Services.AddScoped(services => new BusinessSharkService.ProductDefinitionService.ProductDefinitionServiceClient(invoker));
            builder.Services.AddScoped(services => new BusinessSharkService.ProductCategoryService.ProductCategoryServiceClient(invoker));
            builder.Services.AddScoped(services => new BusinessSharkService.SummaryService.SummaryServiceClient(invoker));

            builder.Services.AddSingleton<GlobalDataProvider>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
