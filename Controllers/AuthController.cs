using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using OfflineMessagingAPI.Settings;
using OfflineMessagingAPI.Services;
using Microsoft.AspNetCore.Authentication;

namespace OfflineMessagingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService; 
        }


        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await _userService.GetUserByUserName(request.Username);
            if (user.Count == 0)
            {
                var newUser = new User
                {
                    Username = request.Username,
                    Password = request.Password,
                };
                await _userService.Register(newUser);
                return Ok();
            }
            else
            {
                return BadRequest("username or pass is invalid");
            }

        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(User user)
        {
            var userName = await _userService.GetUserByUserName(user.Username);
            if (userName.Count == 0)
            {
                return BadRequest("username or pass is invalid");
            }
            else
            {
                return Ok(userName);
            }
        }
        
        [HttpPost("blockUser")]
        public async Task<IActionResult> BlockUser(string userName)
        {
            return Ok(await _userService.BlockUser(userName));
        }
        //private void CreatePassHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        //{
        //    using (var hmac = new HMACSHA512())
        //    {
        //        passwordSalt = hmac.Key;
        //        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

        //    }
        //}
    }
}
