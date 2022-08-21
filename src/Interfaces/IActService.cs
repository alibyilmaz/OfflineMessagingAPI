using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMessagingAPI.Models;

namespace OfflineMessagingAPI.Interfaces
{
    public interface IActService
    {
        Task<List<ActModel>> GetActs(string userName);
        Task<ActModel> AddAct(ActModel act);

    }
}
