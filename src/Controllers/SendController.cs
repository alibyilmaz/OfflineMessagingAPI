using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfflineMessagingAPI.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OfflineMessagingAPI.Helper;
using OfflineMessagingAPI.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OfflineMessagingAPI.Controllers
{
    [ServiceFilter(typeof(TokenFilter))]
    [Route("api/[controller]")]
    [ApiController]
    
    public class SendController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IBlockService _blockService;
        private readonly ILogger<SendController> _logger;
        private readonly IActService _actService;

        public SendController(IMessageService messageService, IUserService userService, IBlockService blockService, ILogger<SendController> logger, IActService actService)
        {
            _messageService = messageService;
            _userService = userService;
            _blockService = blockService;
            _logger = logger;
            _actService = actService;   
        }

        [HttpPost("message")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SendMessage(Messages message, string userName)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                var userFromToken = await _userService.GetUserByToken(token);
                var user = await _userService.GetUserByUserName(userName);
                Guid userId = user.First().Guid;
                Guid userfromTokenId = userFromToken.Guid;
                var validTo = userFromToken.ValidTo;
                message.SenderUser = userFromToken.Username;
                bool IsBlocked = await _blockService.BlockUserCheck(userfromTokenId, userId);
                if (validTo < DateTime.UtcNow.AddMinutes(60) && userFromToken != null && IsBlocked == false)
                {
                    await _messageService.SendMessageByUserName(message, userName);
                    await _actService.AddAct(new ActModel()
                    {
                        Username = userFromToken.Username,
                        Message = $"message sent to {userName}"
                    });
                    return Ok();
                }
                else if (IsBlocked == true)
                {
                    await _actService.AddAct(new ActModel()
                    {
                        Username = userFromToken.Username,
                        Message = $"message not sent to {userName} because you are blocked"
                    });
                    return BadRequest("You are blocked");
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
        [HttpGet("getSendingMessages")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetSendingMessage()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                var userFromToken = await _userService.GetUserByToken(token);
                var userName = userFromToken.Username;
                var validTo = userFromToken.ValidTo;
                if (validTo < DateTime.UtcNow.AddMinutes(60) && userFromToken != null)
                {
                    return Ok(await _messageService.GetSendingMessagesByName(userName));
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
        [HttpGet("getReceivedMessages")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetReceivingMessage()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                var userFromToken = await _userService.GetUserByToken(token);
                var userName = userFromToken.Username;
                var validTo = userFromToken.ValidTo;
                _logger.LogInformation("Executive action AuthController.GetReceivingMessage()");
                if (validTo < DateTime.UtcNow.AddMinutes(60) && userFromToken != null)
                {

                    return Ok(await _messageService.GetReceivingMessagesByName(userName));
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
