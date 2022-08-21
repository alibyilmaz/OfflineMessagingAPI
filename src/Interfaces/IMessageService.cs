using OfflineMessagingAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMessagingAPI.Controllers;

namespace OfflineMessagingAPI.Interfaces
{
    public interface IMessageService
    {
        Task<List<Messages>> GetSendingMessagesByName(string name);
        Task<List<Messages>> GetReceivingMessagesByName(string name);
        Task<Messages> SendMessageByUserName(Messages message, string userName);
    }
}
