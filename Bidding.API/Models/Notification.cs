using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class Notification
    {
        [BsonElement("NotificationId")]
        public string NotificationId { get; set; }

        [BsonElement("SupplierId")]
        public string SupplierId { get; set; }

        [BsonElement("BuyerStatus")]
        public string BuyerStatus { get; set; }

        [BsonElement("SupplierStatus")]
        public string SupplierStatus { get; set; }

        [BsonElement("ParticipationStatus")]
        public string ParticipationStatus { get; set; }

        [BsonElement("Quote")]
        public string Quote { get; set; }
        
        [BsonElement("QuotedAmount")]
        public string QuotedAmount { get; set; }

        [BsonElement("RequestedDocuments")]
        public string RequestedDocuments { get; set; }

        [BsonElement("Supplier")]
        public Supplier Supplier { get; set; }
        [BsonElement("Product")]
        public Product Product { get; set; }
        [BsonElement("AwardedStatus")]
        public string AwardedStatus { get; set; }
        public Notification()
        {
            Supplier = new Supplier();
            Product = new Product();

        }
    }
}
