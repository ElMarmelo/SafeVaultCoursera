using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SafeVault.Web.Services
{
    public class TokenService
    {
        private readonly string _jwtSecret;

        public TokenService()
        {
            _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
        }

        public string GenerateToken(string username, string role)
        {
            var key = Encoding.UTF8.GetBytes(_jwtSecret);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
