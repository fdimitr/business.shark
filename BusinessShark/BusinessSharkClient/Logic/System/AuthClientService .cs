using BusinessSharkService;
using System.IdentityModel.Tokens.Jwt;


namespace BusinessSharkClient.Logic.System
{
    public interface IAuthService
    {
        Task<string> GetValidAccessTokenAsync();
        Task<bool> RefreshTokenAsync();
    }

    public class AuthClientService : IAuthService
    {
        private AuthService.AuthServiceClient _authServiceClient;
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        public AuthClientService(AuthService.AuthServiceClient authServiceClient)
        {
            _authServiceClient = authServiceClient;
        }

        public async Task<string> GetValidAccessTokenAsync()
        {
            var accessToken = SecureStorage.GetAsync("access_token").GetAwaiter().GetResult();

            if (IsTokenExpiringSoon(accessToken))
            {
                await _refreshLock.WaitAsync();
                try
                {
                    // Проверим снова после ожидания — возможно, другой поток уже обновил
                    accessToken = SecureStorage.GetAsync("access_token").GetAwaiter().GetResult();
                    if (IsTokenExpiringSoon(accessToken))
                    {
                        await RefreshTokenAsync();
                    }
                }
                finally
                {
                    _refreshLock.Release();
                }
            }

            accessToken = SecureStorage.GetAsync("access_token").GetAwaiter().GetResult();
            return accessToken ?? string.Empty;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            var refreshToken = SecureStorage.GetAsync("refresh_token").GetAwaiter().GetResult();

            if (string.IsNullOrEmpty(refreshToken))
                return false;

            AuthResponse? response = null;
            try
            {
                response = await _authServiceClient.RefreshTokenAsync(new RefreshRequest { RefreshToken = refreshToken });
            }
            catch (Exception)
            {
                return false;
            }

            if (response == null) return false;

            await SecureStorage.SetAsync("access_token", response.AccessToken);
            await SecureStorage.SetAsync("refresh_token", response.RefreshToken);

            return true;
        }

        private bool IsTokenExpiringSoon(string? token)
        {
            if (string.IsNullOrEmpty(token))
                return true;

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var timeLeft = jwt.ValidTo - DateTime.UtcNow;
            return timeLeft < TimeSpan.FromMinutes(1);
        }
    }
}
