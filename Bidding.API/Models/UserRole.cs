using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class UserRole
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserRoleId")]
        public string UserRoleId { get; set; }

        [BsonElement("UserRoleName")]
        public string UserRoleName { get; set; }

        [BsonElement("IsEdit")]
        public int IsEdit { get; set; }

        [BsonElement("IsBidAvailable")]
        public int IsBidAvailable { get; set; }

        [BsonElement("IsAddAvailable")]
        public int IsAddAvailable { get; set; }

        [BsonElement("IsDelete")]
        public int IsDelete { get; set; }

    }
}
