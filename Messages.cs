using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OfflineMessagingAPI
{
    [Table("messages")]
    public class Messages
    {
        [BsonId]
        public string Id { get; set; }
        [BsonElement("message")]
        public string Message { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [ForeignKey(nameof(UserDto))]
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
    }
}
