
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SolveChess_Auth_API;

public class JWTProvider
{

    private readonly string secretKey = "UeG7K:3xpA:VG@@#YZB{{+Dau#Ar$oaviSx$$?h;`9JSaYi4m]lDnrCK$j,g|X*";
    private readonly string issuer = "SolveChess Authenticator";
    private readonly string audience = "SolveChess Microservice";

    public string GenerateToken(string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

