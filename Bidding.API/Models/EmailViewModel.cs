using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class EmailViewModel
    {
        public string SupplierId { get; set; }
        public string BuyerId { get; set; }
        public string Code { get; set; }            //Includes Product Code or RFQ code
        public string TemplateType { get; set; }
    }
}
