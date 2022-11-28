using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class UserStatus
    {
        public string UserId { get; set; }
        public string Status { get; set; }
        public int DeletedStatus { get; set; }
        public string ModifiedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
