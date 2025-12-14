using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LearningPlatform.Common.Enums;
using LearningPlatform.Common.Options;
using LearningPlatform.Core.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LearningPlatform.API.Services;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly byte[] _keyBytes;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _keyBytes = Encoding.UTF8.GetBytes(_options.Key);
        if (_keyBytes.Length < 32)
        {
            throw new InvalidOperationException("JWT key length must be at least 32 bytes.");
        }
    }

    public (string token, DateTime expiresAtUtc) GenerateToken(Guid userId, string email, UserRole role)
    {
        var expires = DateTime.UtcNow.AddMinutes(_options.ExpiresMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Role, role.ToString())
        };

        var credentials = new SigningCredentials(new SymmetricSecurityKey(_keyBytes), SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (token, expires);
    }
}


