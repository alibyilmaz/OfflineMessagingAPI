using System.Threading.Tasks;
using OfflineMessagingAPI.Models;

namespace OfflineMessagingAPI.Interfaces
{
    public interface IJwtManagerService
    {
        Task<Tokens> Authenticate(User users);

    }
}
