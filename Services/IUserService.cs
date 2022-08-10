using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfflineMessagingAPI.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUserByUserName(string name);

        Task<User> GetUserByUserId(string id);
        Task<User> Register(User user);
        Task<bool> Login(User user);
        Task<bool> BlockUser(string userName);
  
    }
}
