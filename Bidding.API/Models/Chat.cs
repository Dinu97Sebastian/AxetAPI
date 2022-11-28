using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class Chat
    {
        [BsonElement("UserId")]
        public string UserId { get; set; }
        [BsonElement("Message")]
        public string Message { get; set; }
        [BsonElement("Time")]
        public string Time { get; set; }
    }
}
