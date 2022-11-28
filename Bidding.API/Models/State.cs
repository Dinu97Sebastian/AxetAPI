using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class State
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("stateid")]
        public string StateId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("countryid")]
        public string CountryId { get; set; }
    }
}
