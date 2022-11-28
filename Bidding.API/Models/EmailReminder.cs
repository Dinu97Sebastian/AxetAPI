using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bidding.API.Models
{
    public class EmailReminder
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }              

        [BsonElement("RFQId")]
        public string RFQId { get; set; }

        [BsonElement("BiddingDate")]
        public string BiddingDate { get; set; }

        [BsonElement("BiddingStartTime")]
        public string BiddingStartTime { get; set; }

        [BsonElement("EmailId")]
        public string EmailId { get; set; } 

        [BsonElement("ReminderTimes")]
        public List<TimeReminder> ReminderTimes { get; set; }

        public EmailReminder()
        {
            ReminderTimes = new List<TimeReminder>();
        }
    }
}
