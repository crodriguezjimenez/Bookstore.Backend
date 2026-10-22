using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderService.API.Auth;

public record TokenRequest(string ClientId, string ClientSecret);
public record TokenResponse(string AccessToken, string TokenType, int ExpiresIn);

public static class TokenService
{
    
    private static readonly Dictionary<string, (string Secret, string[] Scopes)> RegisteredClients = new()
    {
        // sólo lectura
        ["order-reader"] = ("secret123", new[] { "inventory:read" }),

        //lectura y escritura
        ["order-admin"] = ("adminSecret456", new[] { "inventory:read", "inventory:write" })
    };

    public static TokenResponse? GenerateToken(TokenRequest request, IConfiguration configuration)
    {
        if (!RegisteredClients.TryGetValue(request.ClientId, out var client) || client.Secret != request.ClientSecret)
        {
            return null;
        }

        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.ClientId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        
        foreach (var scope in client.Scopes)
        {
            claims.Add(new Claim("scope", scope));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new TokenResponse(tokenHandler.WriteToken(token), "Bearer", 7200);
    }
}
