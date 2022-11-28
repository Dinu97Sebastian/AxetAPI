using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class DocumentResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("SupplierId")]
        public string SupplierId { get; set; }

        [BsonElement("RequestStatus")]
        public List<UpdateRequestStatus> RequestStatus { get; set;}

        [BsonElement("Files")]
        public List<Document> Files { get; set; }

        [BsonElement("Chat")]
        public List<Chat> Chat { get; set; }

        public DocumentResponse()
        {
            RequestStatus = new List<UpdateRequestStatus>();
            Files = new List<Document>();
            Chat = new List<Chat>();
        }
    }
}
