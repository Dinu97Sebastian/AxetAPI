using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class ChatViewModel
    {
        public string RFQId { get; set; }           //ObjectId of RFQ

        public string SupplierId { get; set; }
        public string UserId { get; set; }

        public string Message { get; set; }

        public string Time { get; set; }
    }
}
