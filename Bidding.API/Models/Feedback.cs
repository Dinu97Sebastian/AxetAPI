using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class Feedback
    {
        [BsonElement("RfqId")]
        public string RfqId { get; set; }

        [BsonElement("SupplierId")]
        public string SupplierId { get; set; }

        [BsonElement("RatingValue")]
        public int RatingValue { get; set; }

        [BsonElement("BuyerId")]
        public string BuyerId { get; set; }

        [BsonElement("BuyerName")]
        public string BuyerName { get; set; }
    }
}
