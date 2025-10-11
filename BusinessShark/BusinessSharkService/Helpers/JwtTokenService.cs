using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BusinessSharkService.Helpers;

public class JwtTokenService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly TimeSpan _tokenLifetime = TimeSpan.FromMinutes(15);

    // This is a simple in-memory store for refresh tokens.
    private readonly Dictionary<string, string> _refreshTokens = new();

    public JwtTokenService(string key, string issuer)
    {
        _key = key;
        _issuer = issuer;
    }

    public (string accessToken, string refreshToken) GenerateTokens(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Name, username),
            new Claim("role", "user")
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            claims: claims,
            expires: DateTime.UtcNow.Add(_tokenLifetime),
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = GenerateRefreshToken();

        lock (_refreshTokens)
        {
            _refreshTokens[refreshToken] = username;
        }

        return (accessToken, refreshToken);
    }

    public (string accessToken, string refreshToken)? RefreshTokens(string refreshToken)
    {
        lock (_refreshTokens)
        {
            if (_refreshTokens.TryGetValue(refreshToken, out var username))
            {
                // Remove old refresh token
                _refreshTokens.Remove(refreshToken);
                return GenerateTokens(username);
            }
        }
        return null;
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}
