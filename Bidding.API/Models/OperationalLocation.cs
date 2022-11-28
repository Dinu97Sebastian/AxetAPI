using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class OperationalLocation
    {
        [BsonElement("Id")]
        public int Id { get; set; }

        [BsonElement("Country")]
        public string Country { get; set; }

        [BsonElement("State")]
        public string State { get; set; }
        [BsonElement("City")]
        public string City { get; set; }

        

        
    }
}
