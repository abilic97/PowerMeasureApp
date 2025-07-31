using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PowerMeasure.Data;
using PowerMeasure.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PowerMeasure.Services
{
    public class TokenHandlerService : ITokenHandlerService
    {
        private readonly IConfiguration configuration;
        private PowerMeasureDbContext _powerMeasureDbContext;

        public TokenHandlerService(IConfiguration configuration, PowerMeasureDbContext contex)
        {
            this.configuration = configuration;
            _powerMeasureDbContext = contex;
            
        }
        public Task<string> CreateTokenAsync(Users user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
            claims.Add(new Claim(ClaimTypes.Surname, user.LastName));
            claims.Add(new Claim(ClaimTypes.Email, user.EmailAddress));
            claims.Add(new Claim(ClaimTypes.Sid, user.Id.ToString()));

            var userRoles = _powerMeasureDbContext.User_Roles.Where(x => x.UserId == user.Id);
            var roles = (from rol in _powerMeasureDbContext.Roles
                        join usr in userRoles on rol.Id equals usr.RoleId
                        select rol.RoleName).ToList();

            roles.ForEach((role) =>
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            });

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              configuration["Jwt:Issuer"],
              configuration["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
