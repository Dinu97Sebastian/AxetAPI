using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class AxetNotification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Content")]
        public string Content { get; set; }

        [BsonElement("Action")]
        public string Action { get; set; }

        [BsonElement("User")]
        public string User { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }

        [BsonElement("Time")]
        public string Time { get; set; }
    }
}
