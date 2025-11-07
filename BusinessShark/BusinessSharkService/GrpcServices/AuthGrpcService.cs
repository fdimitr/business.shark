using BusinessSharkService.Handlers;
using BusinessSharkService.Helpers;
using Grpc.Core;

namespace BusinessSharkService.GrpcServices;

public class AuthGrpcService : AuthService.AuthServiceBase
{
    private readonly JwtTokenService _jwt;
    private readonly PlayerHandler _playerHandler;
    private readonly CompanyHandler _companyHandler;

    public AuthGrpcService(JwtTokenService jwt, PlayerHandler playerHandler, CompanyHandler companyHandler)
    {
        _jwt = jwt;
        _playerHandler = playerHandler;
        _companyHandler = companyHandler;
    }

    public async override Task<AuthResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var player = await _playerHandler.GetByLoginAsync(request.Username);
        if (player != null && PasswordHelper.VerifyPassword(request.Password, player.Password))
        {
            var company = await _companyHandler.GetByPlayer(player.PlayerId);
            if (company == null)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid auth data structure"));

            var (access, refresh) = _jwt.GenerateTokens(request.Username);
            return new AuthResponse
            {
                AccessToken = access,
                RefreshToken = refresh,
                ExpiresIn = DateTime.UtcNow.AddMinutes(15).ToString("O"),
                PlayerId = player.PlayerId,
                CompanyId = company.CompanyId,
                CompanyName = company.Name
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
