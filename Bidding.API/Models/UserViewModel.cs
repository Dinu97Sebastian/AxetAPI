using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class UserViewModel
    {
        public string UserType { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string UserRole { get; set; }

        public string UserId { get; set; }

        public string Status { get; set; }

        public string CreatedDate { get; set; }

        public string ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public int DeletedStatus { get; set; }
    }
}
