using System;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OfflineMessagingAPI.Interfaces;
using OfflineMessagingAPI.Models;
using OfflineMessagingAPI.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfflineMessagingAPI.Services
{
    public class ActService : IActService
    {
        private readonly IMongoCollection<ActModel> _act;
        private readonly ILogger<ActModel> _logger;

        public ActService(IMongoSettings settings, ILogger<ActModel> logger)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
            _act = database.GetCollection<ActModel>(settings.ActCollectionName);
            _logger = logger;
        }
        public async Task<ActModel> AddAct(ActModel act)
        {
            try
            {
                await _act.InsertOneAsync(act);
                return act;
            }
            catch (Exception ex)
            {
                _logger.LogError("an errror ehile seeding database {Error} {StackTrace}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        public async Task<List<ActModel>> GetActs(string userName)
        {
            List<ActModel> acts = new List<ActModel>();
            try
            {
                acts = await _act.Find(act => act.Username == userName).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("an errror ehile seeding database {Error} {StackTrace}", ex.Message, ex.StackTrace);
               
            }
            return acts;
        }
    }
}
