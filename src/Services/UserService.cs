using MongoDB.Driver;
using OfflineMessagingAPI.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using MongoDB.Bson;
using OfflineMessagingAPI.Models;
using System;
using OfflineMessagingAPI.Interfaces;

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

        public async Task UpdateTokenExpireTime(string userName, DateTime validTo)
        {
            //var getUser = await _user.Find(user => user.Username == userName).FirstOrDefaultAsync();
            var filter = Builders<User>.Filter.Eq(u=>u.Username, userName);
            var updateUser = Builders<User>.Update.Set(u=>u.ValidTo, validTo);
            await _user.UpdateOneAsync(filter, updateUser);
        }
        public async Task UpdateToken(string userName, string Token)
        {
            //var getUser = await _user.Find(user => user.Username == userName).FirstOrDefaultAsync();
            var filter = Builders<User>.Filter.Eq(u => u.Username, userName);
            var updateUser = Builders<User>.Update.Set(u => u.Token, Token);
            await _user.UpdateOneAsync(filter, updateUser);
        }
        public async Task<User> Register(User user)
        {
            await _user.InsertOneAsync(user);
            return user;
        }

        public async Task<User> GetUserByToken(string token)
        {

            return await _user.Find(user => user.Token == token).FirstOrDefaultAsync();
        }




    }
}
