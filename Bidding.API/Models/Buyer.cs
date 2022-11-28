using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Bidding.API.Models
{
    public class Buyer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("BuyerId")]
        public string BuyerId { get; set; }

        [BsonElement("BuyerName")]
        public string BuyerName { get; set; }                           //Buyer Name

        [BsonElement("Logo")]
        public string Logo { get; set; }

        [BsonElement("CompanyName")]
        public string CompanyName { get; set; }                         //Buyer Company Name

        [BsonElement("Address")]
        public string Address { get; set; }                             //Buyer Address

        [BsonElement("Country")]
        public string Country { get; set; }                             //Country

        [BsonElement("State")]
        public string State { get; set; }                               //State

        [BsonElement("City")]
        public string City { get; set; }                                //City

        [BsonElement("Zipcode")]
        public string Zipcode { get; set; }                             //Zipcode

        [BsonElement("Email")]
        public string Email { get; set; }                               //Email

        [BsonElement("ContactNumber")]
        public string ContactNumber { get; set; }                       //Buyer Telephone Number

        [BsonElement("Website")]
        public string Website { get; set; }                             //Buyer/Company website

        [BsonElement("Users")]
        public List<UsersList> Users { get; set; }

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

        [BsonElement("Files")]
        public List<UserDocument> Files { get; set; }

        [BsonElement("Feedback")]
        public List<FeedbackResponse> Feedback { get; set; }

        public Buyer()
        {
            Files = new List<UserDocument>();
            Feedback = new List<FeedbackResponse>();
            Users = new List<UsersList>();
        }

    }
}
