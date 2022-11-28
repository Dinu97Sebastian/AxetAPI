using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class PaymentDetails
    {
        [BsonElement("OrderId")]
        public string OrderId { get; set; }
        [BsonElement("PaymentStatus")]
        public string PaymentStatus { get; set; }
        [BsonElement("PaymentId")]
        public string PaymentId { get; set; }
        [BsonElement("Signature")]
        public string Signature { get; set; }
    }
}
