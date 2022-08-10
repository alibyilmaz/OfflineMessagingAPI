using MongoDB.Driver;
using OfflineMessagingAPI.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<List<Messages>> GetMessagesByUserId(string id)
        {
            return await _messages.Find(message => message.UserId == id).ToListAsync();
        }

        public async Task<Messages> SendMessageByUserName(Messages message, string userName)
        {
            
                await _messages.InsertOneAsync(message);
                return message;
            

    }
        
    }
}
