using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Bidding.API.Models
{
    public class Type
    {
        [BsonElement("Id")]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Attributes")]
        public List<Attribute> Attributes { get; set; }

        [BsonElement("RatingFields")]
        public List<RatingField> RatingFields { get; set; }

        public Type()
        {
            Attributes = new List<Attribute>();
            RatingFields = new List<RatingField>();
        }

        
    }
}
