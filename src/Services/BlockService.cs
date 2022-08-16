using System;
using OfflineMessagingAPI.Models;
using System.Threading.Tasks;
using MongoDB.Driver;
using OfflineMessagingAPI.Settings;
using OfflineMessagingAPI.Interfaces;

namespace OfflineMessagingAPI.Services
{
    public class BlockService : IBlockService
    {
        private readonly IMongoCollection<BlockUsers> _blockUsers;
        public BlockService(IMongoSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
            _blockUsers = database.GetCollection<BlockUsers>(settings.BlockUsersCollectionName);
        }

        public async Task<bool> BlockUserCheck(Guid userId, Guid blockedUserId)
        {
            var blockUsers = await _blockUsers.Find(block => block.BlockFromUserID == userId && block.BlockToUserID == blockedUserId).FirstOrDefaultAsync();
            if (blockUsers == null)
            {
                return false;
            }

            return true;
        }
        public async Task<BlockUsers> BlockUser(BlockUsers blockUsers)
        {

            await _blockUsers.InsertOneAsync(blockUsers);
            return blockUsers;
        }
    }
}
