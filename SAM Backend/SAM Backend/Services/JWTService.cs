using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SAM_Backend.Models;
using SAM_Backend.Utility;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
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
                new Claim("UserId", user.Id),
                new Claim("Email", user.Email)
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

        public static string FindEmailByToken(string authorization)
        {
            string token = string.Empty;
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                token = headerValue.Parameter;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var userEmail = securityToken.Claims.First(claim => claim.Type.Equals("Email")).Value;
            return userEmail;
        }

        public async Task<AppUser> FindUserByTokenAsync(HttpRequest request, AppDbContext context)
        {
            string authorization;
            try
            {
                authorization = request.Headers[HeaderNames.Authorization].ToString();
            }
            catch (IndexOutOfRangeException e)
            {
                return null;
            }
            if (authorization.IsNullOrEmpty())
            {
                return null;
            }
            var userEmail = FindEmailByToken(authorization);
            return await context.Users.SingleOrDefaultAsync(x => x.Email == userEmail);
        }

    }
}
