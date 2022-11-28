using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class FeedbackResponse
    {
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Value")]
        public int Value { get; set; }
        [BsonElement("UserId")]
        public string UserId { get; set; }
        [BsonElement("RFQId")]
        public string RFQId { get; set; }
    }
}
