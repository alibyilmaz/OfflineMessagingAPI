using System;
using System.Threading.Tasks;
using OfflineMessagingAPI.Models;

namespace OfflineMessagingAPI.Interfaces
{
    public interface IBlockService
    {
        Task<BlockUsers> BlockUser(BlockUsers blockUsers);

        Task<bool> BlockUserCheck(Guid userId, Guid blockedUserId);
    }
}
