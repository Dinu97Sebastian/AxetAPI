using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class EmailTemplate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("TemplateType")]
        public string TemplateType { get; set; }
        [BsonElement("Subject")]
        public string Subject { get; set; }

        [BsonElement("Body")]
        public string Body { get; set; }


    }
}
