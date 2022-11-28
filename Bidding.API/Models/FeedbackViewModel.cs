using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class FeedbackViewModel
    {
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public string BuyerId { get; set; }
        public string RFQId { get; set; }
        public string SupplierId { get; set; }
    }
}
