using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerAPIPractice.Core.Model;

namespace TaskManagerAPIPractice.Persistence
{
    public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
    {
        private readonly JwtOptions _options = options.Value;

        public string GenerateToken(User user)
        {
            Claim[] claims = [new("userId", user.Id.ToString()), new("email", user.Email)];

            if (string.IsNullOrWhiteSpace(_options.SecretKey))
            {
                throw new Exception("JWT Secret Key is not configured properly.");
            }
            var signingCredential = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256); //Приймає секретний ключ за допомогою якого ми можимо закодувати і розкодувати токен

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredential,
                expires: DateTime.UtcNow.AddHours(4)  //_options.ExpitersHours * 4 некоректно працює
                );

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
    }
}
