using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

namespace BusinessSharkClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            string baseAddress = "http://localhost:5042";

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

            var baseUrl = new Uri(baseAddress);
            var channel = Grpc.Net.Client.GrpcChannel.ForAddress(baseUrl);

            builder.Services.AddScoped(services => new BusinessSharkService.Greeter.GreeterClient(channel));
            builder.Services.AddScoped(services => new BusinessSharkService.AuthService.AuthServiceClient(channel));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
