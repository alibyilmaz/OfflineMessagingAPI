using MongoDB.Driver;
using OfflineMessagingAPI.Models;
using OfflineMessagingAPI.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OfflineMessagingAPI.Interfaces;

namespace OfflineMessagingAPI.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMongoCollection<Messages> _messages;
        private readonly UserService _userService;
        public MessageService(IMongoSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
            _messages = database.GetCollection<Messages>(settings.MessageCollectionName);
        }

        public Task<List<Messages>> GetReceivingMessagesByName(string name)
        {
            return _messages.Find(message => message.ReceiverUser == name).ToListAsync();
        }

        public Task<List<Messages>> GetSendingMessagesByName(string name)
        {
            return _messages.Find(message => message.SenderUser == name).ToListAsync();
        }

        public async Task<Messages> SendMessageByUserName(Messages message, string userName)
        {

          
            var messages = new Messages()
            {
                Message = message.Message,
                SenderUser = message.SenderUser,
                ReceiverUser = userName,

            };
            await _messages.InsertOneAsync(messages);
            return messages;


        }
    }
}
