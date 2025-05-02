using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace game_platform_rwa.Security
{
    public class JwtTokenProvider
    {
        public static string CreateToken(string secureKey, int expiration, string username = null, string role = null)
        {
            var tokenKey = Encoding.UTF8.GetBytes(secureKey);

            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(username))
            {
                claims.Add(new Claim(ClaimTypes.Name, username));
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, username));
            }

            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiration),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);

            return serializedToken;
        }
    }
}
