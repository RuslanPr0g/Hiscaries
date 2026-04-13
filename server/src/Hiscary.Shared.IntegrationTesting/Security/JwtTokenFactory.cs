using Hiscary.Shared.Domain.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hiscary.Shared.IntegrationTesting.Security;

public sealed class JwtTokenFactory
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SigningCredentials _credentials;

    public JwtTokenFactory(string key, string issuer, string audience)
    {
        _issuer = issuer;
        _audience = audience;
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        _credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
    }

    public string CreateToken(string role)
    {
        var claims = new[]
        {
            new Claim(AuthorizationPolicies.RoleClaimType, role),
            new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
            new Claim("id", Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: _credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
