using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class UsersList
    {
        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Role")]
        public string Role { get; set; }

        [BsonElement("Actions")]
        public List<AxetActions> Actions { get; set; }
        public UsersList()
        {
            Actions = new List<AxetActions>();
        }
    }
}
