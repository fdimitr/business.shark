using Microsoft.Extensions.Logging;

namespace BusinessSharkClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            string baseAddress = "https://localhost:7209";

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
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
