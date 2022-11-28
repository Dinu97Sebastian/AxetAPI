using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class DocumentViewModel
    {
        public string RFQId { get; set; }

        public string SupplierId { get; set; }

        [BsonElement("RequestStatus")]
        public List<UpdateRequestStatus> RequestStatus { get; set; }

        [BsonElement("Files")]
        public List<Document> Files { get; set; }
    }
}
