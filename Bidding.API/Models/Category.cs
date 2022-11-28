using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Bidding.API.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        public Category()
        {
            Subcategories = new List<Subcategory>();
        }

        [BsonElement("Subcategories")]
        public List<Subcategory> Subcategories  { get; set; }
    }
}
