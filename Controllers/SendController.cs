using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfflineMessagingAPI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OfflineMessagingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class SendController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public SendController(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService; 
        }
        
        [HttpPost("message")]
        public async Task<IActionResult> SendMessage(Messages message, string userName)
        {
            var user = await _userService.GetUserByUserName(userName);
            if (user.Select(x => x.IsBlocked == true).FirstOrDefault())
            {
                return BadRequest("user is blocked");
            }
            else if (user.Select(x => x.Username != userName).FirstOrDefault())
            {
                return BadRequest("user is not found");
            }
            else
            {
                await _messageService.SendMessageByUserName(message,userName);
                return Ok();
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessageByUserId(string userId)
        {
            return Ok(await _messageService.GetMessagesByUserId(userId));
        }
    }
}
