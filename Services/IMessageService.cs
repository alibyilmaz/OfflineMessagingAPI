using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfflineMessagingAPI.Services
{
    public interface IMessageService
    {
        Task<List<Messages>> GetMessagesByUserId(string id);
        Task<Messages> SendMessageByUserName(Messages message, string userName);
    }
}
