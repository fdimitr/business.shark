using BusinessSharkService.Helpers;
using Grpc.Core;

namespace BusinessSharkService.GrpcServices;

public class AuthGrpcService : AuthService.AuthServiceBase
{
    private readonly JwtTokenService _jwt;

    public AuthGrpcService(JwtTokenService jwt)
    {
        _jwt = jwt;
    }

    public override Task<AuthResponse> Login(LoginRequest request, ServerCallContext context)
    {
        // ⚠️ In real applications, validate credentials against a user store.
        if (request.Username == "admin" && request.Password == "12345")
        {
            var (access, refresh) = _jwt.GenerateTokens(request.Username);
            return Task.FromResult(new AuthResponse
            {
                AccessToken = access,
                RefreshToken = refresh,
                ExpiresIn = DateTime.UtcNow.AddMinutes(15).ToString("O")
            });
        }

        throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid credentials"));
    }

    public override Task<AuthResponse> RefreshToken(RefreshRequest request, ServerCallContext context)
    {
        var tokens = _jwt.RefreshTokens(request.RefreshToken);
        if (tokens == null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid refresh token"));

        return Task.FromResult(new AuthResponse
        {
            AccessToken = tokens.Value.accessToken,
            RefreshToken = tokens.Value.refreshToken,
            ExpiresIn = DateTime.UtcNow.AddMinutes(15).ToString("O")
        });
    }
}
