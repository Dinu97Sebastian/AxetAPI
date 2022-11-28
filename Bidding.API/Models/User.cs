using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bidding.API.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserType")]
        public string UserType { get; set; }

        [BsonElement("UserRole")]
        public string UserRole { get; set; }

        [BsonElement("UserID")]
        public string UserID { get; set; }//userid anand

        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("PasswordHash")]
        public byte[] PasswordHash { get; set; }

        [BsonElement("PasswordSalt")]
        public byte[] PasswordSalt { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }

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

    }
}
