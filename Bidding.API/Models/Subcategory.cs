using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Bidding.API.Models
{
    public class Subcategory
    {
        [BsonElement("Id")]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        public Subcategory()
        {
            Types = new List<Type>();
        }

        [BsonElement("Types")]
        public List<Type> Types { get; set; }
    }
}
