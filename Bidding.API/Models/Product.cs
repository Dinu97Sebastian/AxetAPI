using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Bidding.API.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }                        //Product Name

        [BsonElement("ProductCode")]
        public string ProductCode { get; set; }                 //Product Code

        [BsonElement("Description")]
        public string Description { get; set; }                 //Product Description

        [BsonElement("SupplierId")]
        public string SupplierId { get; set; }                  //Supplier Id to identify the supplier corresponding to the product 

        [BsonElement("Image")]
        public string Image { get; set; }                       //Product Image

        [BsonElement("ImageUrl")]
        public string ImageUrl { get; set; }

        [BsonElement("ImageType")]
        public string ImageType { get; set; }                   //Image Type
        
        [BsonElement("Category")]
        public string Category { get; set; }                    //Product Category

        [BsonElement("Subcategory")]
        public string Subcategory { get; set; }                 //Product Subcategory

        [BsonElement("Type")]
        public string Type { get; set; }                        //Product Type

        [BsonElement("Attributes")]
        public List<Attribute> Attributes{ get; set; }          //Product Attributes

        [BsonElement("Rating")]
        public List<int> Rating { get; set; }                   //Product Rating the Buyer has given

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

        [BsonElement("Feedback")]
        public List<FeedbackResponse> Feedback { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }

        public Product()
        {
            Attributes = new List<Attribute>();
            Feedback = new List<FeedbackResponse>();
        }
    }
}

