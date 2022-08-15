using System;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OfflineMessagingAPI.Models
{
    [Table("blockedUsers")]
    public class BlockUsers
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        [BsonElement(Order = 0)]
        public string Id { get; } = ObjectId.GenerateNewId().ToString();
        [BsonElement("BlockFromUserID")]
        public Guid BlockFromUserID  { get; set; }

        [BsonElement("BlockToUserID")]
        public Guid BlockToUserID  { get; set; }
    }
}
