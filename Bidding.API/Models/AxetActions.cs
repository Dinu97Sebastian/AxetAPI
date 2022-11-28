using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class AxetActions
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ActionName")]
        public string ActionName { get; set; }

        [BsonElement("Permission")]
        public Boolean Permission { get; set; }

        [BsonElement("UserType")]
        public string UserType { get; set; }
    }
}
