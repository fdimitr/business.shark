using BusinessSharkClient.Data;
using BusinessSharkClient.Data.Repositories;
using BusinessSharkClient.Data.Repositories.Interfaces;
using BusinessSharkClient.Data.Sync;
using BusinessSharkClient.Data.Sync.Interfaces;
using BusinessSharkClient.Interceptors;
using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.System;
using CommunityToolkit.Maui;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Licensing;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;

namespace BusinessSharkClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            string baseAddress = "http://10.0.2.2:5042";
            //SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF1cX2hIf0x3Q3xbf1x1ZFxMYV1bR3dPMyBoS35Rc0RiWXZeeXVURWFYVkBxVEFc");
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH5fcXRSRWJcVkVwXkBWYEg=");

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                // Initialize the .NET MAUI Community Toolkit by adding the below line of code
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .UseLiveCharts()
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
                HttpHandler = handler,
            });

            string dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "BusinessSharkData.db"
            );

            builder.Services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlite($"Filename={dbPath}");
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

            builder.Services.AddScoped(services =>
            {
                var invoker = services.GetRequiredService<CallInvoker>();
                return new BusinessSharkService.DivisionWarehouseService.DivisionWarehouseServiceClient(invoker);
            });

            builder.Services.AddScoped(services =>
            {
                var invoker = services.GetRequiredService<CallInvoker>();
                return new BusinessSharkService.DivisionSizeService.DivisionSizeServiceClient(invoker);
            });

            builder.Services.AddScoped(services =>
            {
                var invoker = services.GetRequiredService<CallInvoker>();
                return new BusinessSharkService.CityService.CityServiceClient(invoker);
            });

            builder.Services.AddScoped(services =>
            {
                var invoker = services.GetRequiredService<CallInvoker>();
                return new BusinessSharkService.CountryService.CountryServiceClient(invoker);
            });

            builder.Services.AddScoped(services =>
            {
                var invoker = services.GetRequiredService<CallInvoker>();
                return new BusinessSharkService.ToolsService.ToolsServiceClient(invoker);
            });

            // Repositories
            builder.Services.AddScoped(typeof(ILocalRepository<>), typeof(EfLocalRepository<>));
            builder.Services.AddScoped<DataStateRepository>();

            // Sync Handlers
            builder.Services.AddScoped<ISyncHandler, ProductDefinitionSyncHandler>();
            builder.Services.AddScoped<ISyncHandler, ProductCategorySyncHandler>();
            builder.Services.AddScoped<ISyncHandler, SawmillSyncHandler>();
            builder.Services.AddScoped<ISyncHandler, CitySyncHandler>();
            builder.Services.AddScoped<ISyncHandler, CountrySyncHandler>();
            builder.Services.AddScoped<ISyncHandler, ToolsSyncHandler>();
            builder.Services.AddScoped<SyncEngine>();

            // Providers
            builder.Services.AddSingleton<GlobalDataProvider>();
            builder.Services.AddScoped<SawmillProvider>();
            builder.Services.AddScoped<DivisionTransactionProvider>();
            builder.Services.AddScoped<DivisionWarehouseProvider>();
            builder.Services.AddScoped<DivisionSizeProvider>();
            builder.Services.AddScoped<ToolsProvider>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.ConfigureSyncfusionCore();

            var app = builder.Build();

            // Database initialization
            using var scope = app.Services.CreateScope();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using var db = dbFactory.CreateDbContext();
            DbInitializer.Initialize(db);

            return app;
        }
    }
}
