using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Bidding.API.Models
{
    public class Attribute
    {
        [BsonElement("Id")]
        public string Id { get; set; }

        [BsonElement("Key")]
        public string Key { get; set; }

        [BsonElement("Options")]
        //public List<string> Options { get; set; }
        public List<Options> Options { get; set; }

        [BsonElement("Value")]
        public string Value { get; set; }

        [BsonElement("Optional")]
        public bool Optional { get; set; }
        [BsonElement("Document")]
        public bool Document { get; set; }


        [BsonElement("Component")]
        public string Component { get; set; }       //TextField or Dropdown(Select)

        public Attribute()
        {
            Options = new List<Options>();
        }
    }
}
