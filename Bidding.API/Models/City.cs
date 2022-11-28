using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class City
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("cityid")]
        public string CityId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("stateid")]
        public string StateId { get; set; }
    }
}
