using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class BiddingViewModel
    {
        public string RFQId { get; set; }

        public string SupplierId { get; set; }

        public string Price { get; set; }

        public string Time { get; set; }
    }
}
