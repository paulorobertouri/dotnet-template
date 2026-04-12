using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotnetTemplate.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace DotnetTemplate.Application.Services;

public sealed class AuthService(
    string secret,
    string issuer,
    string audience,
    int expirationSeconds = 3600) : IAuthService
{
    private readonly SymmetricSecurityKey _key =
        new(Encoding.UTF8.GetBytes(secret));

    public string IssueToken(string subject)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, subject),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(expirationSeconds),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string? ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = _key,
                ClockSkew = TimeSpan.Zero,
            }, out _);

            var jwt = handler.ReadJwtToken(token);
            return jwt.Subject;
        }
        catch
        {
            return null;
        }
    }
}
