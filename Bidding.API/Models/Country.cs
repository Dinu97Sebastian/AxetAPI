using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class Country
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("countryid")]
        public int CountryId { get; set; }

        [BsonElement("sortname")]
        public string SortName { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("phoneCode")]
        public int PhoneCode { get; set; }
    }
}
