using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class StatusUpdateViewModel
    {
        public string Id { get; set; }      //Object Id of RFQ

        public string NotificationId { get; set; }

        public string BuyerStatus { get; set; }

        public string SupplierStatus { get; set; }

        public string ParticipationStatus { get; set; }

        public string BiddingStatus { get; set; }

        public string Quote { get; set; }
        public string RequestedDocuments { get; set; }

    }
}
