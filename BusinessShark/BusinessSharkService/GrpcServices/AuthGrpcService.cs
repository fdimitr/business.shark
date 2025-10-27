using BusinessSharkService.Handlers;
using BusinessSharkService.Helpers;
using Grpc.Core;

namespace BusinessSharkService.GrpcServices;

public class AuthGrpcService : AuthService.AuthServiceBase
{
    private readonly JwtTokenService _jwt;
    private readonly PlayerHandler _playerHandler;

    public AuthGrpcService(JwtTokenService jwt, PlayerHandler playerHandler)
    {
        _jwt = jwt;
        _playerHandler = playerHandler;
    }

    public async override Task<AuthResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var player = await _playerHandler.GetByLoginAsync(request.Username);
        if (player != null && PasswordHelper.VerifyPassword(request.Password, player.Password))
        {
            var (access, refresh) = _jwt.GenerateTokens(request.Username);
            return new AuthResponse
            {
                AccessToken = access,
                RefreshToken = refresh,
                ExpiresIn = DateTime.UtcNow.AddMinutes(15).ToString("O"),
                PlayerId = player.PlayerId
            };
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
