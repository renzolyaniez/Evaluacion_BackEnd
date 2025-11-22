using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

class Program
{
    static void Main()
    {
        var secret = "xxxxC3d4E5f6G7h8I9j0K1L2M3n4O5P6Q7r8S9t0U1V2W3x4Y5z6a7B8c9";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var exp = now.AddDays(365);

        var token = new JwtSecurityToken(
            issuer: "custom",
            audience: null,
            claims: new[] { new Claim(JwtRegisteredClaimNames.Sub, "user") },
            notBefore: now,
            expires: exp,
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.WriteToken(token);
        Console.WriteLine(jwt);
        Console.WriteLine("Vence (UTC): " + exp.ToString("o"));
    }
}
