using System;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OfflineMessagingAPI.Models
{
    [Table("acts")]
    public class ActModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        [BsonElement(Order = 0)]
        public string Id { get; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("actDate")]
        public DateTime ActDate { get; set; } = DateTime.UtcNow;
    }
}
