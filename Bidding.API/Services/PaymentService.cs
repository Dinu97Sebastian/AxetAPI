using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Razorpay.Api;
using Bidding.API.Models;
using System.Text;
using MongoDB.Driver;

namespace Bidding.API.Services
{
    public class PaymentService
    {
        private readonly IMongoCollection<RFQ> rfqs;
        public string secret = "gcsbnzh13I5rs7XjeQQebuAV";
        public string key = "rzp_test_omimVhFQGWuXRd";
        
        public PaymentService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            rfqs = database.GetCollection<RFQ>(settings.RFQCollectionName);
        }
        public string GetOrderId(string id, out bool Result)
        {
            try
            {
                RazorpayClient client = new RazorpayClient(key, secret);
                RFQ rfq = rfqs.Find<RFQ>(match => match.Id == id).FirstOrDefault();
                PaymentDetails details = new PaymentDetails();
                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", 500);
                options.Add("receipt", rfq.RFQId);
                options.Add("currency", "INR");
                options.Add("payment_capture", 1);
                Razorpay.Api.Order order = client.Order.Create(options);
                string orderId = order["id"].ToString();
                details.OrderId = orderId;
                rfq.PaymentDetails.Add(details);
                var res = rfqs.ReplaceOne(match => match.Id == id, rfq);
                Result = res.IsAcknowledged;
                return orderId;
            }
            catch(Exception e)
            {
                Result = false;
                return string.Empty;
            }
        }

        public string VerifyPayment(string id, PaymentDetails model)
        {
            RFQ rfq = rfqs.Find<RFQ>(match => match.Id == id).FirstOrDefault();
            var detailsList = rfq.PaymentDetails.Find(match => match.OrderId == model.OrderId);
            detailsList.PaymentId = model.PaymentId;
            detailsList.Signature = model.Signature;
            var res = rfqs.ReplaceOne(match => match.Id == id, rfq);
            var updateResult = res.IsAcknowledged;
            string message = model.OrderId + "|" + model.PaymentId;
            string key = secret;

            byte[] keyByte = new ASCIIEncoding().GetBytes(key);
            byte[] messageBytes = new ASCIIEncoding().GetBytes(message);

            byte[] hashmessage = new HMACSHA256(keyByte).ComputeHash(messageBytes);

            string generated_signature = String.Concat(Array.ConvertAll(hashmessage, x => x.ToString("x2")));

            if (generated_signature == model.Signature && updateResult)
                return "Payment Successful";

            return null;
        }
    }
}
