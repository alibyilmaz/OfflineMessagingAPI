using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using OfflineMessagingAPI.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OfflineMessagingAPI.Models;
using System.Security.Claims;
using System.IO;
using Microsoft.AspNetCore.Identity;
using OfflineMessagingAPI.Helper;
using OfflineMessagingAPI.Interfaces;

namespace OfflineMessagingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
   
        private readonly IUserService _userService;
        private readonly IJwtManagerService _jWTManager;
        private readonly IBlockService _blockService;


        public AuthController(IUserService userService, IJwtManagerService jWtManager, IBlockService blockService)
        {
            _userService = userService;
            _jWTManager = jWtManager;
            _blockService = blockService;   
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await _userService.GetUserByUserName(request.Username);
           
            if (user.Count == 0)
            {
                var newUser = new User
                {
                    Username = request.Username,
                    Password = request.Password,
                    Guid = Guid.NewGuid()
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
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserDto user)
        {
            var userFromDb = await _userService.GetUserByUserName(user.Username);
            if (userFromDb.Count == 0)
            {
                return BadRequest("username or pass is invalid");
            }
            else
            {
                var userLogin = userFromDb.FirstOrDefault();
                if (userLogin.Password == user.Password)
                {
                    List<Claim> claims = new List<Claim> {
                       new Claim("ID",userLogin.Id),
                        new Claim("Name",userLogin.Username),
                    };
                    var token = _jWTManager.Authenticate(userLogin);
                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity)).Wait();
                    return Ok(new { token });

                  
                }
                else
                {
                    return BadRequest("username or pass is invalid");
                }
            }
        }

        [HttpPost("blockUser")]
        [ServiceFilter(typeof(TokenFilter))]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<BlockUsers>> BlockUser(string userName)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var userFromToken = await _userService.GetUserByToken(token);
            var user = await _userService.GetUserByUserName(userName);
            if (user.Count == 0)
            {
                return BadRequest("user not found");
            }
            else
            {
                var userBlock = user.FirstOrDefault();
                var blockUser = new BlockUsers
                {
                    BlockFromUserID = userFromToken.Guid,
                    BlockToUserID = userBlock.Guid
                };
                await _blockService.BlockUser(blockUser);
                return Ok();
            }
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

    public class ClaimNames
    {
        public string Id { get; set; }
    }
}
