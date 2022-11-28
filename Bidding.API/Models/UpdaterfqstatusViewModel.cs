using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class UpdaterfqstatusViewModel
    {
        public string RfqId { get; set; }
        public string Status { get; set; }

        public string ModifiedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
