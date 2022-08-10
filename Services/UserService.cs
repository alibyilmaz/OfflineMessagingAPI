using MongoDB.Driver;
using OfflineMessagingAPI.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using MongoDB.Bson;

namespace OfflineMessagingAPI.Services
{
    public class UserService : IUserService
    {

        private readonly IMongoCollection<User> _user;
        public UserService(IMongoSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
            _user = database.GetCollection<User>(settings.UserCollectionName);
        }

        public async Task<List<User>> GetUserByUserName(string name)
        {
            return await _user.Find(user => user.Username == name).ToListAsync();
        }
        public async Task<User> GetUserByUserId(string id)
        {
            return await _user.Find(user => user.Id == id).FirstOrDefaultAsync();
        }
        public async Task<User> Register(User user)
        {
            await _user.InsertOneAsync(user);
            return user;
        }
        public async Task<bool> Login(User user)
        {
            var userName = await _user.Find(u => u.Username == user.Username).ToListAsync();
            if (userName.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [System.Obsolete]
        public async Task<bool> BlockUser(string userName)
        {
            var user = await _user.Find(u => u.Username == userName).FirstOrDefaultAsync();
            
            if (user.IsBlocked == false)
            {

                user.IsBlocked =true;
                var filter = Builders<User>.Filter.Eq(c => c.Username, userName);
                var update = Builders<User>.Update.Set(user => user.IsBlocked, true);
                await _user.UpdateOneAsync(filter, update);
                return true;
            }
            else
            {
                user.IsBlocked = true;
                var filter = Builders<User>.Filter.Eq(c => c.Username, userName);
                var update = Builders<User>.Update.Set(user => user.IsBlocked, false);
                await _user.UpdateOneAsync(filter, update);
                return true;
            }

        }
    }
}
