using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bidding.API.Models;
using Bidding.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bidding.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[AuthorizePermission]
    public class RFQController : Controller
    {
        private readonly RFQService rfqService;

        public RFQController(RFQService service)
        {
            rfqService = service;
        }

        //To get the list of all RFQs
        [HttpGet]
        public ActionResult<List<RFQ>> Get()
        {
            var rfqList = rfqService.Get();
            //string NowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string NowTime = DateTime.Now.ToUniversalTime().ToString("dd-MM-yyyy HH:mm:ss");
            return Json(new { rfqList, NowTime });
        }

        //To get the list of currencies
        [AuthorizePermission]
        [HttpGet("GetCurrency")]
        public ActionResult<List<Currency>> GetCurrency()
        {
            var currencyData = rfqService.GetCurrency();
            return Json(new { currencyData });
        }

        //To get RFQ details corresponding to the id
        [AuthorizePermission]
        [Route("controller/{id}")]
        [HttpGet("{id:length(24)}", Name = "GetRFQ")]
        public ActionResult Get(string id)
        {
            var rfq = rfqService.Get(id);

            if (rfq == null)
            {
                return NotFound();
            }
            
            return Json(new { rfq });
        }
        //To get the latest RFQId
        [AuthorizePermission]
        [HttpGet("GetRFQId/{userId}")]
        public IActionResult GetRfqId(string userId)
        {
            var code = rfqService.GetRFQId(userId);
            return Json(new { code });
        }

        //To create a RFQ
        [AuthorizePermission]
        [HttpPost]
        public ActionResult<RFQ> Create(RFQ rfq)
        {
            var newRfq = rfqService.Create(rfq);
            //return CreatedAtRoute("GetRFQ", new { id = rfq.Id.ToString() }, rfq);
            return Json(new { data = "Success", RFQ = newRfq });
        }

        //To return the list of all RFQs added by a particular buyer(userid)
        [AuthorizePermission]
        [Route("GetBuyerRFQ/{userid}/{page}/{results}")]
        public ActionResult GetBuyerRFQ(string userid, int page, int results)
        {
            var rfqList = rfqService.GetBuyerRFQ(userid, page, results, out var count);
            //string NowTime = DateTime.Now.ToUniversalTime().ToString("dd-MM-yyyy HH:mm:ss");
            string NowTime = DateTime.Now.ToUniversalTime().ToString("ddd, dd MMM yyy HH':'mm':'ss' GMT'");

            return Json(new { rfqList, NowTime, count });
        }

        //To return the list of RFQs with bidding added by a particular buyer(userid)
        [AuthorizePermission]
        [Route("GetBuyerBiddingRFQ/{userid}/{page}/{results}")]
        public ActionResult GetBuyerBiddingRFQ(string userid, int page, int results)
        {
            var rfqList = rfqService.GetBuyerBiddingRFQ(userid, page, results, out var count);
            string NowTime = DateTime.Now.ToUniversalTime().ToString("ddd, dd MMM yyy HH':'mm':'ss' GMT'");
            return Json(new { rfqList, NowTime, count });
        }

        //To return the list of RFQs received by a particular supplier(userid)
        [AuthorizePermission]
        [Route("GetSupplierRfq/{id}/{page}/{results}")]
        public ActionResult<List<RFQ>> GetSupplierRfq(string id, int page, int results)
        {
            var rfqList = rfqService.GetSupplierRfq(id, page, results, out var count);
            string NowTime = DateTime.Now.ToUniversalTime().ToString("dd-MM-yyyy HH:mm:ss");
            //string NowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            return Json(new { rfqList, NowTime, count });
        }

        //To return the list of RFQs with bidding received by a particular supplier(userid)
        [AuthorizePermission]
        [Route("GetSupplierBiddingRfq/{id}/{page}/{results}")]
        public ActionResult<List<RFQ>> GetSupplierBiddingRfq(string id, int page, int results)
        {
            var rfqList = rfqService.GetSupplierBiddingRfq(id, page, results, out var count);
            string NowTime = DateTime.Now.ToUniversalTime().ToString("dd-MM-yyyy HH:mm:ss");
            //string NowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            return Json(new { rfqList, NowTime, count });
        }

        //To update the RFQ details
        [AuthorizePermission]
        [Route("controller/{id}")]
        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, RFQ rfq)
        {
            if (rfqService.Get(id) == null)
            {
                return NotFound();
            }
            rfqService.Update(id, rfq);
            return Json(new { data = "Success"});
        }

        //To update the various statuses in a particular RFQ
        [AuthorizePermission]
        [HttpPost("UpdateStatus")]
        public IActionResult UpdateStatus([FromBody]StatusUpdateViewModel model)
        {
            var rfq = rfqService.UpdateStatus(model); //.Id, model.NotificationId, model.SupplierStatus, model.BuyerStatus);

            if (rfq == null)
                return Json(new { data = "Error" });
            
            return Json(new { data = "OK" });
        }

        //To get the different times of bidding process for a particular RFQ(id)
        [AuthorizePermission]
        [Route("Time/{id}")]
        public IActionResult GetTime(string id)
        {
           var rfq = rfqService.GetTime(id);

            return Json(new { rfq });
        }

        //To add the bid amount in the RFQ entered by the suppliers 
        [AuthorizePermission]
        [HttpPost("UpdateBiddingDetails")]
        public IActionResult UpdateBiddingDetails([FromBody]BiddingViewModel model)
        {
            var rfq = rfqService.UpdateBiddingDetails(model);
            if (rfq == null)
                return null;

            return Json(new { rfq });
        }

        //To add the details of the requested documents for a particular RFQ
        [AuthorizePermission]
        [HttpPost("UpdateDocumentDetails")]
        public IActionResult UpdateDocumentDetails([FromBody]DocumentViewModel model)
        {
            var rfq = rfqService.UpdateDocumentDetails(model);
            if (rfq == null)
                return null;

            return Json(new { data = "Success", rfq });
            }

        [HttpPost("UpdatePaymentStatus")]
        public IActionResult UpdatePaymentStatus([FromBody]PaymentResultModel model)
        {
            var rfq = rfqService.UpdatePaymentStatus(model);
            if (rfq == null)
                return Json(new { data = "Error" });

            return Json(new { rfq });
        }

        [AuthorizePermission]
        [HttpGet("GetFeedbackMaster/{userType}")]
        public IActionResult GetFeedbackMaster(string userType)
        {
            var feedbackMaster = rfqService.GetFeedbackMaster(userType);
            if (feedbackMaster == null)
                return Json(new { data = "Error" });
            return Json(new { feedbackMaster });
        }

        [HttpPost("UpdateBuyerFeedback")]
        public IActionResult UpdateBuyerFeedback([FromBody]FeedbackViewModel model)
        {
            var rfq = rfqService.UpdateBuyerFeedback(model);
            if (rfq == null)
                return Json(new { data = "Error" });
            return Json(new { data = "Success", rfq });
        }

        [HttpPost("UpdateSupplierFeedback")]
        public IActionResult UpdateSupplierFeedback([FromBody]FeedbackViewModel model)
        {
            var buyer = rfqService.UpdateSupplierFeedback(model);
            if (buyer == null)
                return Json(new { data = "Error" });
            return Json(new { data = "Success", buyer });
        }

        [HttpPost("UpdateAwardedStatus")]
        public IActionResult UpdateAwardedStatus([FromBody]AwardedViewModel model)
        {
            var rfq = rfqService.UpdateAwardedStatus(model);
            if (rfq == null)
                return Json(new { data = "Error" });
            return Json(new { data = "Success", rfq });
        }
        [AuthorizePermission]
        [HttpGet("GetBiddingHistory/{id}")]
        public IActionResult GetBiddingHistory(string id)
        {
            var biddingData = rfqService.GetBiddingHistory(id);
            if (biddingData == null)
                return Json(new { data = "Error" });
            return Json(new { biddingData });
        }


        [AuthorizePermission]
        [HttpPost("UpdateChatDetails")]
        public IActionResult UpdateChatDetails([FromBody]ChatViewModel model)
        {
            var rfq = rfqService.UpdateChatDetails(model);
            if (rfq == null)
                return Json(new { data = "Error" });
            return Json(new { data = "Success" });
        }
        [AuthorizePermission]
        [HttpPost("UpdateRfqStatus")]
        public IActionResult UpdateRfqStatus([FromBody]UpdaterfqstatusViewModel model)
        {
            var rfq = rfqService.UpdateRfqStatus(model);
            if (rfq == null)
                return Json(new { data = "Error" });
            return Json(new { data = "Success", RFQ = rfq });
        }
        [Route("controller/{id}")]
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var rfq = rfqService.Get(id);
            if (rfq == null)
            {
                return NotFound();
            }
            //rfqService.Remove(rfq.Id);
            return NoContent();
        }

        [AuthorizePermission]
        [HttpGet]
        [Route("SendAllSupplierNotification/{id}/{act}")]
        public IActionResult SendAllSupplierNotification(string id, string act)
        {
            var Data = rfqService.SendAllSupplierNotification(id, act);
            if (Data == null)
                return Json(new { data = "Error" });
            return Json(new { Data });
        }

        [AuthorizePermission]
        [HttpGet]
        [Route("SendSingleUserNotification/{id}/{act}")]
        public IActionResult SendSingleNotification(string id, string act)
        {
            var Data = rfqService.SendSingleNotification(id, act);
            if (Data == null)
                return Json(new { data = "Error" });
            return Json(new { Data });
        }
        [AuthorizePermission]
        [HttpPost("SendSingleUserNotification")]
        public IActionResult SendSingleNotification([FromBody]AxetNotification model)
        {
            var Data = rfqService.SendSingleNotification(model);
            if (Data == null)
                return Json(new { data = "Error" });
            return Json(new { Data });
        }

    }
}