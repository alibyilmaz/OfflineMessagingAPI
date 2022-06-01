using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace OfflineMessagingAPI
{
    public class UserDto
    {

        public string Username { get; set; } = string.Empty;
   
        public string Password { get; set; } = string.Empty;



    }
}
