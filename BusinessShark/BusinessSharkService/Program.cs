using BusinessSharkService.Constants;
using BusinessSharkService.DataAccess;
using BusinessSharkService.Helpers;
using BusinessSharkService.GrpcServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using BusinessSharkService.CoreServices;
using BusinessSharkService.Handlers;
using BusinessSharkService.Handlers.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

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
// ==================================

builder.Services.AddSingleton(new JwtTokenService(jwtKey, jwtIssuer));
builder.Services.AddGrpc();

builder.Services.AddScoped<IWorldHandler, WorldHandler>();
builder.Services.AddHostedService<CalculationService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

// Configure the HTTP request pipeline.
app.MapGrpcService<AuthGrpcService>().EnableGrpcWeb();
app.MapGrpcService<GreeterService>().EnableGrpcWeb();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
