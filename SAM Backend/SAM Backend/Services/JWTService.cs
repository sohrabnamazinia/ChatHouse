using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SAM_Backend.Models;
using SAM_Backend.Utility;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SAM_Backend.Services
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration configuration;

        public JWTService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string GenerateToken(AppUser user)
        {
            var TokenSignKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>(Constants.TokenSignKey)));
            var Creds = new SigningCredentials(TokenSignKey, SecurityAlgorithms.HmacSha512Signature);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName),
                new Claim("UserId", user.Id)
            };

            var TokenHandler = new JwtSecurityTokenHandler();
            var TokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddYears(1),
                SigningCredentials = Creds
            };

            var Token = TokenHandler.CreateToken(TokenDescriptor);

            return "Bearer " + TokenHandler.WriteToken(Token);
        }
    }
}
