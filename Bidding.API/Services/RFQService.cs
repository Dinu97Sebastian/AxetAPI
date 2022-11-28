using Bidding.API.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using RestSharp;

namespace Bidding.API.Services
{
    public class RFQService
    {
        private readonly IMongoCollection<RFQ> rfqs;
        private readonly IMongoCollection<Currency> currencies;
        private readonly IMongoCollection<Product> products;
        private readonly IMongoCollection<Supplier> suppliers;
        private readonly IMongoCollection<Buyer> buyers;
        private readonly IMongoCollection<FeedbackMaster> feedbacks;
        private readonly IMongoCollection<Sequence> sequence;
        private readonly IMongoCollection<EmailReminder> emailReminders;
        private readonly IMongoCollection<AxetNotification> axetNotification;

        public RFQService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            rfqs = database.GetCollection<RFQ>(settings.RFQCollectionName);
            sequence = database.GetCollection<Sequence>(typeof(Sequence).Name);
            currencies = database.GetCollection<Currency>(settings.CurrencyCollectionName);
            products = database.GetCollection<Product>(settings.ProductCollectionName);
            suppliers = database.GetCollection<Supplier>(settings.SupplierCollectionName);
            buyers = database.GetCollection<Buyer>(settings.BuyerCollectionName);
            feedbacks = database.GetCollection<FeedbackMaster>(settings.FeedbackMasterCollectionName);
            emailReminders = database.GetCollection<EmailReminder>(settings.EmailRemindersCollectionName);
            axetNotification = database.GetCollection<AxetNotification>(settings.AxetNotification);
        }

        //To list all RFQs
        public List<RFQ> Get() =>
            rfqs.Find(rfq => true).ToList();

        //To get the list of currencies
        public List<Currency> GetCurrency()
        {
            IEnumerable<Currency> currency = currencies.Find(currency => true).ToList();
            return currency.ToList();
        }

        //To get RFQ details corresponding to the id
        public List<RFQ> Get(string id)
        {
            IEnumerable<RFQ> rfq = rfqs.Find<RFQ>(match => match.Id == id).ToList();
            return rfq.ToList();
        }

        //To get the latest RFQId
        public string GetRFQId(string userId)
        {
            string value = GetSequenceValue("RfqId").ToString();
            string code = value + userId;
            //RFQ rfq = rfqs.Find<RFQ>(rfq => true).Limit(1).Sort("{$natural:-1}").FirstOrDefault();
            //if (rfq == null)
            //    return "RFQ0001";
            //string code = rfq.RFQId;

            //string code = GetSequenceValue("RFQId").ToString();
            return code;
        }

        public int GetSequenceValue(string sequenceName)
        {
            var filter = Builders<Sequence>.Filter.Eq(s => s.SequenceName, sequenceName);
            var update = Builders<Sequence>.Update.Inc(s => s.SequenceValue, 1);

            var result = sequence.FindOneAndUpdate(filter, update, new FindOneAndUpdateOptions<Sequence, Sequence>
            { IsUpsert = true, ReturnDocument = ReturnDocument.After });

            return result.SequenceValue;
        }

        //To create a RFQ
        public RFQ Create(RFQ rfq)
        {
            string Id = rfq.BuyerId;
            if (rfq.IsBidding == "Not Available")
            {
                rfq.BiddingDate = null;
                rfq.BiddingStartTime = null;
                rfq.BiddingEndTime = null;
            }
            UpdateRFQDetail(rfq);
            rfqs.InsertOne(rfq);

            Buyer buyer = buyers.Find<Buyer>(match => match.BuyerId == Id).FirstOrDefault();
            string BuyerName = buyer.BuyerName;
            CreateNotification(BuyerName + " has created a RFQ", "RfqVarification", "admin");
            return rfq;
        }

        //To return the list of all RFQs added by a particular buyer(userid)
        public List<RFQ> GetBuyerRFQ(string id, int page, int results, out int count)
        {
            int startIndex = (page - 1) * results;
            var rfqData = from r in rfqs.Find(rfq => true).ToList()
                          where r.BuyerId == id && r.DeletedStatus == 0
                          select new RFQ
                          {
                              Id = r.Id,
                              RFQId = r.RFQId,
                              BuyerId = r.BuyerId,
                              Country = r.Country,
                              State = r.State,
                              City = r.City,
                              BiddingDate = r.BiddingDate,
                              CreatedDate = r.CreatedDate,
                              Status = r.Status,
                              RFQStatus = r.RFQStatus,
                              IsBidding = r.IsBidding,
                              Description = r.Description,
                              BiddingStatus = r.BiddingStatus,
                              PaymentStatus = r.PaymentStatus,
                              BiddingStartTime = r.BiddingStartTime,
                              BiddingEndTime = r.BiddingEndTime,
                              Notification = (from n in r.Notification
                                              select new Notification
                                              {
                                                  NotificationId = n.NotificationId,
                                                  SupplierId = n.SupplierId,
                                                  SupplierStatus = n.SupplierStatus,
                                                  ParticipationStatus = n.ParticipationStatus,
                                                  AwardedStatus = n.AwardedStatus,
                                                  BuyerStatus = n.BuyerStatus
                                              }
                                            ).ToList()

                          };
            count = rfqData.Count();
            return rfqData.OrderByDescending(rfq => DateTime.Parse(rfq.CreatedDate)).Skip(startIndex).Take(results).ToList();
        }

        //To return the list of RFQs with bidding added by a particular buyer(userid)
        public List<RFQ> GetBuyerBiddingRFQ(string id, int page, int results, out int count)
        {
            int startIndex = (page - 1) * results;
            var rfqData = from r in rfqs.Find(rfq => true).ToList()
                          where r.BuyerId == id && r.IsBidding == "Available" && r.DeletedStatus == 0 
                          select new RFQ
                          {
                              Id = r.Id,
                              RFQId = r.RFQId,
                              BuyerId = r.BuyerId,
                              Country = r.Country,
                              State = r.State,
                              City = r.City,
                              BiddingDate = r.BiddingDate,
                              CreatedDate = r.CreatedDate,
                              Status = r.Status,
                              RFQStatus = r.RFQStatus,
                              IsBidding = r.IsBidding,
                              Description = r.Description,
                              BiddingStatus = r.BiddingStatus,
                              PaymentStatus = r.PaymentStatus,
                              BiddingStartTime = r.BiddingStartTime,
                              BiddingEndTime = r.BiddingEndTime,
                              Notification = (from n in r.Notification
                                              select new Notification
                                              {
                                                  NotificationId = n.NotificationId,
                                                  SupplierId = n.SupplierId,
                                                  SupplierStatus = n.SupplierStatus,
                                                  ParticipationStatus = n.ParticipationStatus,
                                                  AwardedStatus = n.AwardedStatus,
                                                  BuyerStatus = n.BuyerStatus
                                              }
                                            ).ToList()

                          };

            count = rfqData.Count();
            return rfqData.OrderByDescending(rfq => DateTime.Parse(rfq.BiddingDate)).Skip(startIndex).Take(results).ToList();
        }

        //To return the list of all RFQs received by a particular supplier(userid)
        public List<RFQ> GetSupplierRfq(string id, int page, int results, out int count)
        {
            int startIndex = (page - 1) * results;
            var rfqData = from r in rfqs.Find(rfq => true).ToList()
                          where r.Status == "Active" && r.DeletedStatus == 0
                          select new RFQ
                          {
                              Id=r.Id,
                              RFQId=r.RFQId,
                              BuyerId=r.BuyerId,
                              BiddingStatus=r.BiddingStatus,
                              BiddingStartTime=r.BiddingStartTime,
                              BiddingEndTime=r.BiddingEndTime,
                              BiddingDate=r.BiddingDate,
                              IsBidding=r.IsBidding,
                              Country=r.Country,
                              State=r.State,
                              City=r.City,
                              CreatedDate=r.CreatedDate,
                              Notification= (from n in r.Notification
                                             select new Notification
                                             {
                                                 NotificationId = n.NotificationId,
                                                 SupplierId = n.SupplierId,
                                                 SupplierStatus = n.SupplierStatus,
                                                 ParticipationStatus = n.ParticipationStatus,
                                                 AwardedStatus = n.AwardedStatus,
                                                 BuyerStatus = n.BuyerStatus
                                             }
                                            ).ToList()

                          };
            List<RFQ> supplierRfq = new List<RFQ>();
            foreach (var rfq in rfqData)
            {
                var notification = from n in rfq.Notification.ToList()
                                   select new Notification
                                   {
                                       NotificationId=n.NotificationId,
                                       SupplierId=n.SupplierId,
                                       SupplierStatus=n.SupplierStatus,
                                       ParticipationStatus=n.ParticipationStatus,
                                       AwardedStatus=n.AwardedStatus,
                                       BuyerStatus=n.BuyerStatus
                                   };
                foreach(var note in notification)
                {
                    if(note.SupplierId == id)
                    {
                        supplierRfq.Add(rfq);
       
                    }
                }
            }
            count = supplierRfq.Count();
            return supplierRfq.OrderByDescending(rfq => DateTime.Parse(rfq.CreatedDate)).Skip(startIndex).Take(results).ToList();
        }

        //To return the list of RFQs with bidding received by a particular supplier(userid)
        public List<RFQ> GetSupplierBiddingRfq(string id, int page, int results, out int count)
        {
            int startIndex = (page - 1) * results;
            var rfqData = from r in rfqs.Find(rfq => true).ToList()
                          where r.IsBidding == "Available" && r.Status == "Active" && r.DeletedStatus == 0
                          select new RFQ
                          {
                              Id = r.Id,
                              RFQId = r.RFQId,
                              BuyerId = r.BuyerId,
                              BiddingStatus = r.BiddingStatus,
                              BiddingStartTime = r.BiddingStartTime,
                              BiddingEndTime = r.BiddingEndTime,
                              BiddingDate = r.BiddingDate,
                              IsBidding = r.IsBidding,
                              Country = r.Country,
                              State = r.State,
                              City = r.City,
                              CreatedDate = r.CreatedDate,
                              Notification = (from n in r.Notification
                                              select new Notification
                                              {
                                                  NotificationId = n.NotificationId,
                                                  SupplierId = n.SupplierId,
                                                  SupplierStatus = n.SupplierStatus,
                                                  ParticipationStatus = n.ParticipationStatus,
                                                  AwardedStatus = n.AwardedStatus,
                                                  BuyerStatus = n.BuyerStatus
                                              }
                                            ).ToList()

                          };
            List<RFQ> supplierRfq = new List<RFQ>();
            foreach (var rfq in rfqData)
            {
                var notification = from n in rfq.Notification.ToList()
                                   select new Notification
                                   {
                                       NotificationId = n.NotificationId,
                                       SupplierId = n.SupplierId,
                                       SupplierStatus = n.SupplierStatus,
                                       ParticipationStatus = n.ParticipationStatus,
                                       AwardedStatus = n.AwardedStatus,
                                       BuyerStatus = n.BuyerStatus
                                   };
                foreach (var note in notification)
                {
                    if (note.SupplierId == id)
                    {
                        supplierRfq.Add(rfq);

                    }
                }
            }
            count = supplierRfq.Count();
            return supplierRfq.OrderByDescending(rfq => DateTime.Parse(rfq.CreatedDate)).Skip(startIndex).Take(results).ToList();
        }



        public RFQ UpdateStatus(StatusUpdateViewModel model)
        {
            RFQ rfq = rfqs.Find<RFQ>(match => match.Id == model.Id).FirstOrDefault();
            EmailReminder data = new EmailReminder();
            IEnumerable<Notification> notification = new List<Notification>();
            var note = rfq.Notification.FirstOrDefault(n => n.NotificationId == model.NotificationId);
            string email = suppliers.Find<Supplier>(match => match.SupplierId == note.SupplierId).FirstOrDefault().Email;
                
            List<string> times = new List<string>();
            rfq.BiddingStatus = model.BiddingStatus;
            note.SupplierStatus = model.SupplierStatus;
            note.BuyerStatus = model.BuyerStatus;
            note.RequestedDocuments = model.RequestedDocuments;
            note.ParticipationStatus = model.ParticipationStatus;
            note.Quote = model.Quote;
            rfqs.ReplaceOne(match => match.Id == model.Id, rfq);
            if(rfq.IsBidding == "Available" && note.BuyerStatus == "Accepted" && note.SupplierStatus == "Accepted" && rfq.BiddingStatus == "Pending")
            {
                data.RFQId = rfq.RFQId;
                data.BiddingDate = rfq.BiddingDate;
                data.BiddingStartTime = rfq.BiddingStartTime;
                data.EmailId = email;
                data.ReminderTimes.Add(new TimeReminder { 
                    Time = DateTimeOffset.Parse(rfq.BiddingStartTime).UtcDateTime.AddHours(-4).ToString("ddd, dd MMM yyy HH':'mm':'ss' GMT'"),
                    Status = null
                });
                data.ReminderTimes.Add(new TimeReminder
                {
                    Time = DateTimeOffset.Parse(rfq.BiddingStartTime).UtcDateTime.AddHours(-2).ToString("ddd, dd MMM yyy HH':'mm':'ss' GMT'"),
                    Status = null
                });
                data.ReminderTimes.Add(new TimeReminder
                {
                    Time = DateTimeOffset.Parse(rfq.BiddingStartTime).UtcDateTime.AddMinutes(-15).ToString("ddd, dd MMM yyy HH':'mm':'ss' GMT'"),
                    Status = null
                });
                emailReminders.InsertOne(data);
            }
            return rfq;
        }
      

        public RFQ UpdateBiddingDetails(BiddingViewModel model)
        {
            RFQ rfq = rfqs.Find<RFQ>(match => match.Id == model.RFQId).FirstOrDefault();                        //Here objectId of RFQ is used to get the corresponding RFQ
            var notification = rfq.Notification.FirstOrDefault(n => n.SupplierId == model.SupplierId);
            notification.ParticipationStatus = "participated";
            notification.QuotedAmount = model.Price;
            List<BiddingInfo> biddingData = rfq.Biddings;
            BiddingInfo newBiddingData = new BiddingInfo();
            newBiddingData.SupplierId = model.SupplierId;
            newBiddingData.Price = model.Price;
            newBiddingData.Time = model.Time;
            biddingData.Add(newBiddingData);
            rfq.Biddings = biddingData;
            rfqs.ReplaceOne(match => match.Id == model.RFQId, rfq);
            return rfq;
        }
        public RFQ UpdateDocumentDetails(DocumentViewModel model)
        {
            RFQ rfq = rfqs.Find<RFQ>(match => match.Id == model.RFQId).FirstOrDefault();                        //Here objectId of RFQ is used to get the corresponding 
            List<DocumentResponse> documentData = rfq.Response;
            DocumentResponse newResponse = new DocumentResponse();
            newResponse.SupplierId = model.SupplierId;
            newResponse.RequestStatus = model.RequestStatus;
            newResponse.Files = model.Files;
            if (documentData.Count(doc => doc.SupplierId == model.SupplierId) == 0)
            {
                documentData.Add(newResponse);
                rfq.Response = documentData;
            }
            else
            {
                DocumentResponse tempResponse = documentData.Find(t => t.SupplierId == model.SupplierId);
                tempResponse.Files = model.Files;
                if (model.RequestStatus.Count() != 0)
                {
                    List<UpdateRequestStatus> tempRequestStatus = tempResponse.RequestStatus;
                    foreach(var item in model.RequestStatus)
                    {
                        var request = tempRequestStatus.Find(r => r.Key == item.Key);
                        request.Value = item.Value;
                    }
                    
                }
            }
            rfqs.ReplaceOne(match => match.Id == model.RFQId, rfq);
            return rfq;
        }

        public List<BiddingInfo> GetBiddingHistory(string id)
        {
            var biddingData = rfqs.Find(match => match.Id == id && match.DeletedStatus == 0)
                .ToEnumerable()
                .SelectMany(rfq => rfq.Biddings);

            return biddingData.ToList();
        }

        public List<FeedbackMaster> GetFeedbackMaster(string userType)
        {
            var feedbackData = feedbacks.Find<FeedbackMaster>(match => match.UserType == userType).ToList();
            return feedbackData;
        }

        public Dictionary<string, int> GetBuyerBiddingHistory(string id)
        {
            var biddingData = rfqs.Find(match => match.Id == id && match.DeletedStatus == 0)
                .ToEnumerable()
                .SelectMany(rfq => rfq.Biddings)
                .GroupBy(r => r.SupplierId)
                .ToDictionary(b => b.Key, b => b.Count());

            return biddingData;
        }

        public RFQ UpdatePaymentStatus(PaymentResultModel model)
        {
            RFQ rfq = rfqs.Find<RFQ>(match => match.Id == model.Id).FirstOrDefault();
            var detail = rfq.PaymentDetails.Find(match => match.OrderId == model.OrderId);
            if(!string.IsNullOrWhiteSpace(model.OrderId) && detail?.OrderId == model.OrderId)
            {
                detail.PaymentStatus = "paid";
                rfq.RFQStatus = "completed";
                rfq.PaymentStatus = "paid";
                List<Notification> notifications = rfq.Notification;
                foreach (var note in notifications)
                {
                    Supplier supplier = suppliers.Find<Supplier>(match => match.SupplierId == note.SupplierId).FirstOrDefault();
                    note.Supplier = supplier;
                }
                rfqs.ReplaceOne(match => match.Id == model.Id, rfq);
            }
            return rfq;
        }

        public RFQ UpdateBuyerFeedback(FeedbackViewModel model)
        {
            Product product = products.Find<Product>(match => match.ProductCode == model.ProductCode).FirstOrDefault();
            RFQ rfq = rfqs.Find<RFQ>(match => match.Id == model.RFQId).FirstOrDefault();
            var notification = rfq.Notification.Find(n => n.SupplierId == model.SupplierId);
            List<FeedbackResponse> feedbackList = product.Feedback;
            FeedbackResponse feedbackData = new FeedbackResponse();
            feedbackData.UserId = model.BuyerId;
            feedbackData.Name = model.Name;
            feedbackData.Value = model.Value;
            feedbackData.RFQId = model.RFQId;
            if (feedbackList.Count(f => f.Name == model.Name && f.UserId == model.BuyerId && f.RFQId == model.RFQId) == 0)
            {
                feedbackList.Add(feedbackData);
                product.Feedback = feedbackList;
            }
            else
            {
                FeedbackResponse temp = feedbackList.Find(t => t.Name == model.Name && t.UserId == model.BuyerId && t.RFQId == model.RFQId);
                temp.Value = model.Value;
            }
            notification.Product.Feedback = feedbackList.FindAll(f => f.RFQId == model.RFQId);
            products.ReplaceOne(match => match.ProductCode == model.ProductCode, product);
            rfqs.ReplaceOne(match => match.Id == model.RFQId, rfq);
            return rfq;
        }

        public Buyer UpdateSupplierFeedback(FeedbackViewModel model)
        {
            Buyer buyer = buyers.Find<Buyer>(match => match.BuyerId == model.BuyerId).FirstOrDefault();
            FeedbackResponse response = new FeedbackResponse();
            response.UserId = model.SupplierId;
            response.Name = model.Name;
            response.Value = model.Value;
            response.RFQId = model.RFQId;
            if (buyer.Feedback.Count(f => f.Name == model.Name && f.UserId == model.SupplierId && f.RFQId == model.RFQId) == 0)
            {
                buyer.Feedback.Add(response);
            }
            else
            {
                FeedbackResponse temp = buyer.Feedback.Find(t => t.Name == model.Name && t.UserId == model.SupplierId && t.RFQId == model.RFQId);
                temp.Value = model.Value;
            }
            //buyer.Feedback.Add(response);
            buyers.ReplaceOne(match => match.BuyerId == model.BuyerId, buyer);
            return buyer;
        }

        public RFQ UpdateAwardedStatus(AwardedViewModel model)
        {
            RFQ rfq = rfqs.Find<RFQ>(match => match.Id == model.RFQId).FirstOrDefault();
            var notification = rfq.Notification.Find(note => note.SupplierId == model.SupplierId);
            notification.AwardedStatus = "Awarded";
            rfqs.ReplaceOne(match => match.Id == model.RFQId, rfq);
            return rfq;
        }

        public void Update(string id, RFQ rfq)
        {
            UpdateRFQDetail(rfq);
            rfqs.ReplaceOne(match => match.Id == id, rfq);
        }

        public void Remove(RFQ rfq) =>
            rfqs.DeleteOne(match => match.Id == rfq.Id);

        public void Remove(string id) =>
            rfqs.DeleteOne(match => match.Id == id);

        private void UpdateRFQDetail(RFQ rfq)
        {
            var filter = new BsonDocument("Category", rfq.Category);
            filter.Merge(new BsonDocument("Subcategory", rfq.Subcategory));
            filter.Merge(new BsonDocument("Type", rfq.Type));
            MongoQueryAll query = new MongoQueryAll("Attributes");
            MongoQueryElement element = new MongoQueryElement();
            BsonDocument document = new BsonDocument();
            foreach (var attribute in rfq.Attributes)
            {
                query = new MongoQueryAll("Attributes");
                element = new MongoQueryElement();
                element.QueryPredicates.Add(new MongoQueryPredicate("Key", attribute.Key, "E"));
                element.QueryPredicates.Add(new MongoQueryPredicate("Value", attribute.Value, "E"));
                query.QueryElements.Add(element);
                document = MongoDB.Bson.Serialization
                    .BsonSerializer.Deserialize<BsonDocument>(query.ToString());
                filter.Merge(document);
            }
            List<Supplier> filteredSuppliers = suppliers.Find(supplier => true).ToList();
            OperationalLocation location = rfq.Location.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(location.Country) &&
                !string.IsNullOrWhiteSpace(location.State) &&
                !string.IsNullOrWhiteSpace(location.City))
            {
                filteredSuppliers = filteredSuppliers.SelectMany(x => x.OperationalLocation
                                                .Where(z => (z.Country == location.Country && string.IsNullOrWhiteSpace(z.State) && string.IsNullOrWhiteSpace(z.City)) ||
                                                  (z.Country == location.Country && z.State == location.State && string.IsNullOrWhiteSpace(z.City)) ||
                                                  (z.Country == location.Country && z.State == location.State && z.City == location.City)),
                                                (supplierData, locationData) =>
                                                new
                                                {
                                                    supplierData.SupplierId
                                                }
                                              ).Select(x => new Supplier { SupplierId = x.SupplierId }).ToList();
            }
           else if (!string.IsNullOrWhiteSpace(location.Country) &&
                !string.IsNullOrWhiteSpace(location.State) &&
                string.IsNullOrWhiteSpace(location.City))
            {
                filteredSuppliers = filteredSuppliers.SelectMany(x => x.OperationalLocation
                                                .Where(z => (z.Country == location.Country && string.IsNullOrWhiteSpace(z.State) && string.IsNullOrWhiteSpace(z.City)) ||
                                                  (z.Country == location.Country && z.State == location.State && string.IsNullOrWhiteSpace(z.City))),
                                                (supplierData, locationData) =>
                                                new
                                                {
                                                    supplierData.SupplierId
                                                }
                                              ).Select(x => new Supplier { SupplierId = x.SupplierId }).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(location.Country) &&
               string.IsNullOrWhiteSpace(location.State) &&
               string.IsNullOrWhiteSpace(location.City))
            {
                filteredSuppliers = filteredSuppliers.SelectMany(x => x.OperationalLocation
                                                .Where(z => (z.Country == location.Country && string.IsNullOrWhiteSpace(z.State) && string.IsNullOrWhiteSpace(z.City))),
                                                (supplierData, locationData) =>
                                                new
                                                {
                                                    supplierData.SupplierId
                                                }
                                              ).Select(x => new Supplier { SupplierId = x.SupplierId }).ToList();
            }
            else
            {
                filteredSuppliers = new List<Supplier>();
            }
            foreach (var product in products.Find(filter).ToList())
            {
                Notification notification = new Notification() { NotificationId = Guid.NewGuid().ToString() };
                Supplier supplier = filteredSuppliers.Find(match => match.SupplierId == (product.SupplierId));
                if (supplier != null)
                {
                    notification.SupplierId = product.SupplierId;
                    notification.Supplier.SupplierId = supplier.SupplierId;
                }
                else
                {
                    notification.Supplier = null;
                }
                if (product.Status == "InActive")
                {
                    notification.Product = null;
                }
                else
                {
                    product.Feedback = new List<FeedbackResponse>();
                    notification.Product = product;
                }
                //notification.Product = product;
                notification.BuyerStatus = "New";
                notification.SupplierStatus = "Pending";
                if (notification.Supplier != null && !rfq.Notification.Exists(match => match.Supplier.SupplierId == notification.Supplier.SupplierId) && notification.Product != null)
                    rfq.Notification.Add(notification);
            }
        }

        public Dictionary<string, string> GetTime(string id)
        {
            RFQ rfq = rfqs.Find<RFQ>(match => match.Id == id).FirstOrDefault();
            Dictionary<string, string> time = new Dictionary<string, string>();
            //string biddingDate = rfq.BiddingDate;
            DateTime bidDate = DateTime.Parse(rfq.BiddingDate);
            DateTime date1 = bidDate.Date;
            string biddingDate = date1.ToString("dd-MM-yyyy");
            
            string StartTime = rfq.BiddingStartTime;
            string EndTime = rfq.BiddingEndTime;
            DateTime NowTime = DateTime.Now.ToUniversalTime();
            //string NowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string CurrentTimeDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
            //string NowTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
            DateTime datevalue1 = Convert.ToDateTime(EndTime).ToUniversalTime();
            DateTime datevalue2 = Convert.ToDateTime(NowTime).ToUniversalTime();

            var hours = (datevalue1 - NowTime).TotalMilliseconds;

            //var hours = datevalue1.Subtract(datevalue2);
            string diff = Convert.ToString(hours);
            time.Add("biddingDate", biddingDate);
            time.Add("StartTime", StartTime);
            time.Add("EndTime", EndTime);
            time.Add("CurrentTimeDate", NowTime.ToString("dd-MM-yyyy HH:mm:ss"));
            time.Add("difference", diff);
            return time;
        }

        public RFQ UpdateChatDetails(ChatViewModel model)
        {
            RFQ rfq = rfqs.Find<RFQ>(match => match.Id == model.RFQId).FirstOrDefault();                        //Here objectId of RFQ is used to get the corresponding RFQ
            var document = rfq.Response.FirstOrDefault(n => n.SupplierId == model.SupplierId);
            List<Chat> chatList = document.Chat;
            Chat chatData = new Chat();
            chatData.UserId = model.UserId;
            chatData.Message = model.Message;
            chatData.Time = model.Time;
            chatList.Add(chatData);
            document.Chat = chatList;
            rfqs.ReplaceOne(match => match.Id == model.RFQId, rfq);
            return rfq;
        }
        public RFQ UpdateRfqStatus(UpdaterfqstatusViewModel model)
        {
            RFQ rfq = rfqs.Find<RFQ>(match => match.Id == model.RfqId).FirstOrDefault();                        //Here objectId of RFQ is used to get the corresponding RFQ
            rfq.Status = model.Status;
            rfq.UpdatedBy = model.UpdatedBy;
            rfq.ModifiedDate = model.ModifiedDate;
           
            rfqs.ReplaceOne(match => match.Id == model.RfqId, rfq);
            return rfq;
        }

        public string SendAllSupplierNotification(string Id,string action)
        {
            var Key = "298900AVEDGq5tN7eM5da55d241";                //API KEY
            var Sender = "AXET";                                 //SENDER
            string content = "";
            RFQ rfq = rfqs.Find<RFQ>(match => match.Id == Id).FirstOrDefault();

            DateTime bidDate = DateTime.Parse(rfq.BiddingAcceptDeadline).ToLocalTime();
            string formatted = bidDate.ToString("dd-MM-yyyy");
            var Message = "You received new RFQ: " + rfq.RFQId + ", Response Date:" + formatted;
            if (action == "NewRFQ")
            {
                content = "You received new RFQ: " + rfq.RFQId;
            }

            List<Notification> nots = rfq.Notification.ToList();

            foreach (Notification r in nots)
            {
                CreateNotification(content, action, r.SupplierId);
                Supplier supplier = suppliers.Find<Supplier>(match => match.SupplierId == r.SupplierId).FirstOrDefault();
                var client = new RestSharp.RestClient("" +
                  "https://api.msg91.com/api/sendhttp.php?" +
                  "&mobiles=" + supplier.ContactNumber + "&authkey=" + Key + "" +
                  "&route=1&sender=AXET&message=" + Message + "&country=91");
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
            }
            return "success";
        }
        public string SendSingleNotification(string Id, string action)
        {
            string content = "";
            if(action== "supplierAccept")
            {
                content = "Another supplier accepted your RFQ";
            }
            if (action == "supplierReject")
            {
                content = "Another supplier Rejected your RFQ";
            }

            CreateNotification(content, action, Id);
            return "Success";
        }
        public string SendSingleNotification(AxetNotification model)
        {

            axetNotification.InsertOne(model);
            return "Success";
        }

        public string CreateNotification(string content, string action, string user)
        {
            AxetNotification Nots = new AxetNotification();
            Nots.Content = content;
            Nots.Action = action;
            Nots.User = user;
            Nots.Status = "Unread";
            Nots.Time = DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            axetNotification.InsertOne(Nots);
            return "success";
        }

    }
}
