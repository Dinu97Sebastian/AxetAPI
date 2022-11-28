using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class BiddingInfo
    {
        [BsonElement("SupplierId")]
        public string SupplierId { get; set; }

        [BsonElement("Price")]
        public string Price { get; set; }

        [BsonElement("Time")]
        public string Time { get; set; }

        public BiddingInfo()
        {
           

        }
    }
}
