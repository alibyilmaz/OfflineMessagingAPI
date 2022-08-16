using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfflineMessagingAPI.Settings
{
    public class MongoSettings : IMongoSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get ; set; }
        public string UserCollectionName { get ; set; }
        public string MessageCollectionName { get ; set; }
        public string BlockUsersCollectionName { get ; set; }
    }
}
