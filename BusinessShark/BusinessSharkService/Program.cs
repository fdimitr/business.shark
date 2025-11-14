using System.Security.Claims;
using System.Text;
using BusinessSharkService;
using BusinessSharkService.Constants;
using BusinessSharkService.CoreServices;
using BusinessSharkService.DataAccess;
using BusinessSharkService.GrpcServices;
using BusinessSharkService.GrpcServices.Interceptors;
using BusinessSharkService.Handlers;
using BusinessSharkService.Handlers.Context;
using BusinessSharkService.Handlers.Divisions;
using BusinessSharkService.Handlers.Finance;
using BusinessSharkService.Handlers.Interfaces;
using BusinessSharkService.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//For Entity Framework DbContext
DbContextOptions<DataContext> dbContextOptions = new DbContextOptionsBuilder<DataContext>()
    .UseNpgsql(builder.Configuration.GetValue<string>(ConfigKey.ConnectionString)).Options;

builder.Services.AddSingleton(dbContextOptions);
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetValue<string>(ConfigKey.ConnectionString));
});


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
if (string.IsNullOrEmpty(jwtIssuer))
{
    throw new InvalidOperationException("JWT Issuer is not configured.");
}

// Add services to the container.
// === Setup JWT authentication ===
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        NameClaimType = ClaimTypes.Name,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<LoggingInterceptor>();
});
// ==================================

builder.Services.AddSingleton(new JwtTokenService(builder.Configuration, jwtKey, jwtIssuer));
builder.Services.AddSingleton<IWorldContext, WorldContext>();

builder.Services.AddScoped<WorldHandler>();
builder.Services.AddScoped<ProductDefinitionHandler>();
builder.Services.AddScoped<ProductCategoryHandler>();
builder.Services.AddScoped<SummaryHandler>();
builder.Services.AddScoped<CountryHandler>();
builder.Services.AddScoped<CityHandler>();
builder.Services.AddScoped<DistributionCenterHandler>();
builder.Services.AddScoped<MineHandler>();
builder.Services.AddScoped<SawmillHandler>();
builder.Services.AddScoped<FactoryHandler>();
builder.Services.AddScoped<PlayerHandler>();
builder.Services.AddScoped<CompanyHandler>();
builder.Services.AddScoped<DivisionTransactionHandler>();
builder.Services.AddScoped<WarehouseHandler>();
builder.Services.AddScoped<WarehouseProductsHandler>();
builder.Services.AddScoped<DivisionSizeHandler>();

builder.Services.AddHostedService<CalculationService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

// Configure the HTTP request pipeline.
app.MapGrpcService<DivisionSizeGrpcService>().EnableGrpcWeb();
app.MapGrpcService<DivisionWarehouseGrpcService>().EnableGrpcWeb();
app.MapGrpcService<DivisionTransactionGrpcService>().EnableGrpcWeb();
app.MapGrpcService<SawmillGrpcService>().EnableGrpcWeb();
app.MapGrpcService<CompanyGrpService>().EnableGrpcWeb();
app.MapGrpcService<SummaryGrpcService>().EnableGrpcWeb();
app.MapGrpcService<ProductCategoryGrpService>().EnableGrpcWeb();
app.MapGrpcService<ProductDefinitionGrpcService>().EnableGrpcWeb();
app.MapGrpcService<AuthGrpcService>().EnableGrpcWeb();
app.MapGrpcService<GreeterService>().EnableGrpcWeb();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

