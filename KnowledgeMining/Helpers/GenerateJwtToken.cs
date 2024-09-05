using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KnowledgeMining.Models;
using Microsoft.IdentityModel.Tokens;

namespace KnowledgeMining.Helpers
{
    public class JwtTokenGenerator
    {
        private readonly Jwt _Jwt;

        public JwtTokenGenerator(Jwt jwt)
        {
            _Jwt = jwt;
        }

        public string GenerateToken(User user, string roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, roles)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Jwt.key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _Jwt.Issuer,
                audience: _Jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_Jwt.DurationInDays * 24 * 60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}