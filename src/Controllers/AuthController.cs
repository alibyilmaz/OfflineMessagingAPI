using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OfflineMessagingAPI.Models;
using System.Security.Claims;
using OfflineMessagingAPI.Helper;
using OfflineMessagingAPI.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace OfflineMessagingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IJwtManagerService _jWTManager;
        private readonly IBlockService _blockService;
        private readonly ILogger<AuthController> _logger;
        private readonly IActService _actService;


        public AuthController(IUserService userService, IJwtManagerService jWtManager, IBlockService blockService, ILogger<AuthController> logger, IActService actService)
        {
            _userService = userService;
            _jWTManager = jWtManager;
            _blockService = blockService;
            _logger = logger;
            _actService = actService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string userName, string password)
        {
            try
            {
                var user = await _userService.GetUserByUserName(userName);
                if (user.Count == 0)
                {
                    var newUser = new User
                    {
                        Username = userName,
                        Password = password,
                        Guid = Guid.NewGuid()
                    };
                    await _userService.Register(newUser);
                    await _actService.AddAct(new ActModel()
                    {
                        Message = "user registered",
                        Username = newUser.Username

                    });
                    return Ok();
                }
                else
                {
                    await _actService.AddAct(new ActModel()
                    {
                        Message = "user not registered",
                        Username = userName

                    });
                    return BadRequest("username or pass is invalid");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("an errror while seeding database {Error} {StackTrace}", ex.Message, ex.StackTrace);
                return BadRequest(ex.Message);
            }


        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string userName, string password)
        {
            try
            {
                var userFromDb = await _userService.GetUserByUserName(userName);
                
                if (userFromDb.Count == 0)
                {
                    await _actService.AddAct(new ActModel()
                    {
                        Message = "user not found",
                        Username = userName

                    });
                    return BadRequest("user not found");
                }
                else
                {
                    var userLogin = userFromDb.FirstOrDefault();
                    if (userLogin.Password == password)
                    {
                        List<Claim> claims = new List<Claim> {
                            new Claim("ID",userLogin.Id),
                            new Claim("Name",userLogin.Username),
                        };
                        var token = _jWTManager.Authenticate(userLogin);
                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity)).Wait();
                        await _actService.AddAct(new ActModel()
                        {
                            Message = "user logged in",
                            Username = userLogin.Username

                        });
                        return Ok(new { token });


                    }
                    else
                    {
                        await _actService.AddAct(new ActModel()
                        {
                            Message = "user not logged in",
                            Username = userLogin.Username

                        });

                        return BadRequest("username or pass is invalid");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("an errror while seeding database {Error} {StackTrace}", ex.Message, ex.StackTrace);
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("blockUser")]
        [ServiceFilter(typeof(TokenFilter))]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<BlockUsers>> BlockUser(string userName)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                var userFromToken = await _userService.GetUserByToken(token);
                var user = await _userService.GetUserByUserName(userName);
                if (user.Count == 0)
                {
                    await _actService.AddAct(new ActModel()
                    {
                        Message = "user not found",
                        Username = userFromToken.Username,
                    });
                    return BadRequest("user not found");
                }
                else
                {

                    var userBlock = user.FirstOrDefault();
                    LogContext.PushProperty("User", userFromToken.Username);

                    var blockUser = new BlockUsers
                    {
                        BlockFromUserID = userFromToken.Guid,
                        BlockToUserID = userBlock.Guid
                    };
                    await _blockService.BlockUser(blockUser);
                    await _actService.AddAct(new ActModel()
                    {
                        Message = "user blocked",
                        Username = userFromToken.Username,
                    });
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("an errror while seeding database {Error} {StackTrace}", ex.Message, ex.StackTrace);
                return BadRequest(ex.Message);
            }

        }
    }

}
