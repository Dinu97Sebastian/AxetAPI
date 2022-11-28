using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bidding.API.Models;
using Razorpay;
using Razorpay.Api;
using Bidding.API.Services;


namespace Bidding.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[AuthorizePermission]
    public class PaymentController : Controller
    {
        private readonly PaymentService paymentService;

        public PaymentController(PaymentService service)
        {
            paymentService = service;
        }

        [HttpGet("GetOrderId/{id}")]
        public string GetOrderId(string id)
        {
            var orderId = paymentService.GetOrderId(id, out var result);
            if (result == true) 
                return orderId;
            return null;
        }

        [HttpPost("VerifyPayment/{id}")]
        public IActionResult VerifyPayment(string id, [FromBody]PaymentDetails model)
        {
            var result = paymentService.VerifyPayment(id, model);
            if (result == "Payment Successful")
                return Json(new { data = "Payment Successful" });
            return null;
        }
    }
}