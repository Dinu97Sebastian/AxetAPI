using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class RFQ
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("RFQId")]
        public string RFQId { get; set; }                        //RFQ Id

        [BsonElement("BuyerId")]
        public string BuyerId { get; set; }                      //RFQ Code

        [BsonElement("Description")]
        public string Description { get; set; }                  //RFQ Description

        [BsonElement("Currency")]
        public string Currency { get; set; }                     //Currency

        [BsonElement("IsBidding")]
        public string IsBidding { get; set; }                    //IsBidding

        [BsonElement("NumberOfParticulars")]
        public string NumberOfParticulars { get; set; }          //Number Of Particulars

        [BsonElement("BiddingDate")]
        public string BiddingDate { get; set; }                  //Bidding Date

        [BsonElement("BiddingStartTime")]
        public string BiddingStartTime { get; set; }             //Bidding Start Time

        [BsonElement("BiddingEndTime")]
        public string BiddingEndTime { get; set; }               //Bidding End Time

        [BsonElement("RequiredPeriodStart")]
        public string RequiredPeriodStart { get; set; }          //Required Period Start

        [BsonElement("RequiredPeriodEnd")]
        public string RequiredPeriodEnd { get; set; }            //Required Period End

        [BsonElement("BiddingAcceptDeadline")]
        public string BiddingAcceptDeadline { get; set; }        //Bidding Accept Deadline

        [BsonElement("DocumentDeadlineDate")]
        public string DocumentDeadlineDate { get; set; }         //Document Deadline Date

        [BsonElement("Address")]
        public string Address { get; set; }                      //Address

        [BsonElement("Country")]
        public string Country { get; set; }                      //Country

        [BsonElement("State")]
        public string State { get; set; }                        //State

        [BsonElement("City")]
        public string City { get; set; }                         //City

        [BsonElement("Latitude")]
        public string Latitude { get; set; }
        [BsonElement("Longitude")]
        public string Longitude { get; set; }

        [BsonElement("Category")]
        public string Category { get; set; }                    //Product Category

        [BsonElement("Subcategory")]
        public string Subcategory { get; set; }                 //Product Subcategory

        [BsonElement("Type")]
        public string Type { get; set; }                        //Product Type

        [BsonElement("Attributes")]
        public List<Attribute> Attributes { get; set; }          //Product Attributes

        [BsonElement("Location")]
        public List<OperationalLocation> Location { get; set; }

        [BsonElement("LocationString")]
        public string LocationString { get; set; }

        [BsonElement("CreatedDate")]
        public string CreatedDate { get; set; }

        [BsonElement("ModifiedDate")]
        public string ModifiedDate { get; set; }

        [BsonElement("CreatedBy")]
        public string CreatedBy { get; set; }

        [BsonElement("UpdatedBy")]
        public string UpdatedBy { get; set; }

        [BsonElement("DeletedStatus")]
        public int DeletedStatus { get; set; }

        [BsonElement("RFQStatus")]
        public string RFQStatus { get; set; }

        [BsonElement("BiddingStatus")]
        public string BiddingStatus { get; set; }

        [BsonElement("Notification")]
        public List<Notification> Notification { get; set; }

        [BsonElement("Biddings")]
        public List<BiddingInfo> Biddings { get; set; }
        [BsonElement("Response")]
        public List<DocumentResponse> Response { get; set; }

        [BsonElement("PaymentStatus")]
        public string PaymentStatus { get; set; }
        [BsonElement("PaymentDetails")]
        public List<PaymentDetails> PaymentDetails { get; set; }
        [BsonElement("Status")]
        public string Status { get; set; }
        public RFQ()
        {
            Attributes = new List<Attribute>();
            Location = new List<OperationalLocation>();
            Notification = new List<Notification>();
            Biddings = new List<BiddingInfo>();
            Response = new List<DocumentResponse>();
            PaymentDetails = new List<PaymentDetails>();
        }
    }
}
