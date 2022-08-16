using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfflineMessagingAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
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


        public SendController(IMessageService messageService, IUserService userService, IBlockService blockService)
        {
            _messageService = messageService;
            _userService = userService;
            _blockService = blockService;   
        }

        [HttpPost("message")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SendMessage(Messages message, string userName)
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
                return Ok();
            }
            else if (IsBlocked == true)
            {
                return BadRequest("You are blocked");
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("getSendingMessages")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetSendingMessage()
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
        [HttpGet("getReceivedMessages")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetReceivingMessage()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var userFromToken = await _userService.GetUserByToken(token);
            var userName = userFromToken.Username;
            var validTo = userFromToken.ValidTo;
            if (validTo < DateTime.UtcNow.AddMinutes(60) && userFromToken != null)
            {

                return Ok(await _messageService.GetReceivingMessagesByName(userName));
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
