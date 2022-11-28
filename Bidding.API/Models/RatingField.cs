using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class RatingField
    {
        [BsonElement("Id")]
        public string Id { get; set; }

        [BsonElement("Key")]
        public string Key { get; set; }

        [BsonElement("Value")]
        public int Value { get; set; }
    }
}
