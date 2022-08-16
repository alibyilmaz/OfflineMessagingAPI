using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using OfflineMessagingAPI.Interfaces;
using OfflineMessagingAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMessagingAPI.Services
{
    public class JwtManagerService : IJwtManagerService
    {
        private readonly IConfiguration iconfiguration;
        private readonly IUserService _userService;

        public JwtManagerService(IConfiguration iconfiguration, IUserService userService)
        {
            this.iconfiguration = iconfiguration;
            _userService = userService;
        }

        public async Task<Tokens> Authenticate(User users)
        {
            var user = await _userService.GetUserByUserName(users.Username);
            if (user.Count == 0)
            {
                return null;
            }
            else
            {
                if (user.FirstOrDefault().Password == users.Password)
                {
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(iconfiguration["Jwt:Key"]));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                    var tokeOptions = new JwtSecurityToken(
                        issuer: iconfiguration["Jwt:Issuer"],
                        audience: iconfiguration["Jwt:Audience"],
                        claims: new List<Claim>(),
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: signinCredentials
                    );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                    
                    var tokens = new Tokens()
                    {
                        Token = tokenString,
                        Expiration = tokeOptions.ValidTo
                    };
               
                    await _userService.UpdateTokenExpireTime(users.Username, tokens.Expiration);
                    await _userService.UpdateToken(users.Username, tokens.Token);
                    return tokens;
                }
                else
                {
                    return null;
                }
            }
        }
        
    }
}
