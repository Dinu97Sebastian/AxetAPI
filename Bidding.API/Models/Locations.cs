using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class Locations
    {
        [BsonElement("Location")]
        public string Location { get; set; }
    }
}
