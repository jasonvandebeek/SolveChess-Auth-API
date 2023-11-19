
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SolveChess.Logic;

public class JwtProvider
{

    private readonly string _secretKey;
    private readonly string _issuer = "SolveChess Authenticator";
    private readonly string _audience = "SolveChess API";

    public JwtProvider(string secretKey)
    {
        _secretKey = secretKey;
    }

    public string GenerateToken(string userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("Id", userId),
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

