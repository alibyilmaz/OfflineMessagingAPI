using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfflineMessagingAPI.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OfflineMessagingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly ILogger<HistoryController> _logger;
        private readonly IActService _actService;
        private readonly IUserService _userService;

        public HistoryController(ILogger<HistoryController> logger, IActService actService, IUserService userService)
        {
            _logger = logger;
            _actService = actService;
            _userService = userService; 
        }
        // GET: api/<HistoryController>
        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                var userFromToken = await _userService.GetUserByToken(token);
                var userName = userFromToken.Username;
                var validTo = userFromToken.ValidTo;
                if (validTo < DateTime.UtcNow.AddMinutes(60) && userFromToken != null)
                {
                    var acts = await _actService.GetActs(userName);
                    return Ok(acts);
                }
                else
                {
                    return Unauthorized();
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
