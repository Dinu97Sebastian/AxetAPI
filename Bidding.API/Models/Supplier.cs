using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Bidding.API.Models
{
    public class Supplier
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("SupplierId")]
        public string SupplierId { get; set; }

        [BsonElement("SupplierName")]
        public string SupplierName { get; set; }                        //Supplier Name

        [BsonElement("Logo")]
        public string Logo { get; set; }                        //Supplier Name

        [BsonElement("CompanyName")]
        public string CompanyName { get; set; }                         //Supplier Company Name

        [BsonElement("Address")]
        public string Address { get; set; }                             //Supplier Address

        [BsonElement("Country")]
        public string Country { get; set; }                             //Country to be included

        [BsonElement("State")]
        public string State { get; set; }                               //State to be included

        [BsonElement("City")]
        public string City { get; set; }                                //City to be included

        [BsonElement("Zipcode")]
        public string Zipcode { get; set; }                             //Zipcode

        [BsonElement("Email")]
        public string Email { get; set; }                               //Supplier Email

        //[BsonElement("Password")]
        //public string Password { get; set; }                            //Password used for login by the supplier

        [BsonElement("ContactNumber")]
        public string ContactNumber { get; set; }                       //Supplier Telephone Number

        [BsonElement("Website")]
        public string Website { get; set; }                             //Supplier/Company website

        [BsonElement("OperationalLocation")]
        public List<OperationalLocation> OperationalLocation { get; set; }

        public List<Locations> Locations { get; set; }

        [BsonElement("RatingValue")]
        public int RatingValue { get; set; }
        public List<UsersList> Users { get; set; }                         //For additional user roles

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

        public Supplier()
        {
            OperationalLocation = new List<OperationalLocation>();
            Locations = new List<Locations>();
            Files = new List<UserDocument>();
            Users = new List<UsersList>();
        }

        

    }
}
