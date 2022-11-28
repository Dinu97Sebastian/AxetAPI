using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class Document
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("File")]
        public string File { get; set; }

        [BsonElement("FileType")]
        public string FileType { get; set; }
        [BsonElement("FileName")]
        public string FileName { get; set; }
    }
}
