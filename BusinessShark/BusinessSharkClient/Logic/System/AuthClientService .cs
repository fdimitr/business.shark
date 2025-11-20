using BusinessSharkService;
using System.IdentityModel.Tokens.Jwt;


namespace BusinessSharkClient.Logic.System
{
    public interface IAuthService
    {
        Task<string> GetValidAccessTokenAsync();
        Task<bool> RefreshTokenAsync();
    }

    public class AuthClientService(AuthService.AuthServiceClient authServiceClient) : IAuthService
    {
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        public async Task<string> GetValidAccessTokenAsync()
        {
            var accessToken = await SecureStorage.GetAsync("access_token").ConfigureAwait(false);

            if (IsTokenExpiringSoon(accessToken))
            {
                await _refreshLock.WaitAsync();
                try
                {
                    // Проверим снова после ожидания — возможно, другой поток уже обновил
                    accessToken = await SecureStorage.GetAsync("access_token").ConfigureAwait(false);
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

            accessToken = await SecureStorage.GetAsync("access_token").ConfigureAwait(false);
            return accessToken ?? string.Empty;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            var refreshToken = await SecureStorage.GetAsync("refresh_token").ConfigureAwait(false);

            if (string.IsNullOrEmpty(refreshToken))
                return false;

            AuthResponse? response = null;
            try
            {
                response = await authServiceClient.RefreshTokenAsync(new RefreshRequest { RefreshToken = refreshToken });
            }
            catch (Exception)
            {
                return false;
            }

            if (response == null) return false;

            await SecureStorage.SetAsync("access_token", response.AccessToken).ConfigureAwait(false);
            await SecureStorage.SetAsync("refresh_token", response.RefreshToken).ConfigureAwait(false);

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
