using OfflineMessagingAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfflineMessagingAPI.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetUserByUserName(string name);

        Task<User> GetUserByUserId(string id);
        Task<User> Register(User user);

        Task UpdateTokenExpireTime(string userName, DateTime validTo);
        Task UpdateToken(string userName, string token);
        Task<User> GetUserByToken(string token);

    }
}
