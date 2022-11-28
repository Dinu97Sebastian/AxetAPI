using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using Bidding.API.Models;
using MongoDB.Driver;
using System.Globalization;

namespace Bidding.API.Services
{
    public class DashboardService
    {
        private readonly IMongoCollection<RFQ> rfqs;
        private readonly IMongoCollection<Product> products;
        private readonly IMongoCollection<Supplier> suppliers;
        private readonly IMongoCollection<Buyer> buyers;

        public DashboardService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            rfqs = database.GetCollection<RFQ>(settings.RFQCollectionName);
            products = database.GetCollection<Product>(settings.ProductCollectionName);
            suppliers = database.GetCollection<Supplier>(settings.SupplierCollectionName);
            buyers = database.GetCollection<Buyer>(settings.BuyerCollectionName);
        }

        //Returns the total number of products corresponding to Supplier Id
        public Dictionary<string, int> GetProductCount(string id)
        {
            var count = (int)products.Find(p => p.SupplierId == id && p.DeletedStatus == 0)
                .CountDocuments();
            var productCount = new Dictionary<string, int> {
                { "total", count }
            };
            return productCount;
        }

        //Returns the RFQ counts corresponding to Supplier Id
        public Dictionary<string, int> GetSupplierRfqCount(string id)
        {
            var items = rfqs.Find(r => r.DeletedStatus == 0 && r.Notification.Any(n => n.SupplierId == id))
                .ToEnumerable();
            Dictionary<string, int> count = new Dictionary<string, int>();
            count.Add("Total", items.Count());                                              //Total number of RFQs corresponding to Supplier
            count.Add("Accepted", items.Sum(r => r.Notification.Count(n => n.SupplierId == id && n.SupplierStatus == "Accepted")));  //Total number of RFQs accepted by the Supplier
            count.Add("Participated", items
                .Where(r => r.IsBidding == "Available")
                .Sum(r => r.Notification.Count(n => n.SupplierId == id && n.ParticipationStatus == "participated"))); //Total number of RFQ bids in which the Supplier participated
            count.Add("BidsAwarded", items
                .Sum(r => r.Notification.Count(n => n.SupplierId == id && n.AwardedStatus == "Awarded")));

            return count;
        }

        //Returns the total number of RFQs for each product corresponding to Supplier Id
        public Dictionary<string, int> GetProductRfqCount(DashboardViewModel model)
        {
            Dictionary<string, int> count = new Dictionary<string, int>();
            if(model.Month == "All")
            {
                count = rfqs
                    .Find(r => r.DeletedStatus == 0 && r.Notification.Any(n => n.SupplierId == model.Id)) // filtering inside mongo db
                    .ToEnumerable()
                    .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("yyyy") == model.Year)
                    .SelectMany(r => r.Notification.Where(n => n.SupplierId == model.Id))
                    .Select(n => n.Product.ProductCode)
                    .GroupBy(p => p)
                    .ToDictionary(p => p.Key, p => p.Count());
            }
            else
            {
                count = rfqs
                    .Find(r => r.DeletedStatus == 0 && r.Notification.Any(n => n.SupplierId == model.Id))
                    .ToEnumerable()
                    .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("yyyy") == model.Year)
                    .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("MMMM") == model.Month)
                    .SelectMany(r => r.Notification.Where(n => n.SupplierId == model.Id))
                    .Select(n => n.Product.ProductCode)
                    .GroupBy(p => p)
                    .ToDictionary(p => p.Key, p => p.Count());
            }
            return count;
        }

        //Returns the count of RFQs appeared for each category
        public Dictionary<string, int> GetCategoryRfqCount(DashboardViewModel model)
        {
            Dictionary<string, int> categoryCount = new Dictionary<string, int>();
            if (model.Month == "All")
            {
                categoryCount = rfqs.Find(r => r.DeletedStatus == 0)
                .ToEnumerable()
                .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("yyyy") == model.Year)
                .Select(r => r.Category)
                .GroupBy(c => c)
                .ToDictionary(c => c.Key, c => c.Count());
            }
            else
            {
                categoryCount = rfqs.Find(r => r.DeletedStatus == 0)
                .ToEnumerable()
                .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("yyyy") == model.Year)
                .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("MMMM") == model.Month)
                .Select(r => r.Category)
                .GroupBy(c => c)
                .ToDictionary(c => c.Key, c => c.Count());
            }
            return categoryCount;
        }

        public Dictionary<string, int> GetBuyerRfqCount(string id)
        {
            var rfqList = rfqs.Find<RFQ>(match => match.BuyerId == id && match.DeletedStatus == 0)
                .ToEnumerable();
            Dictionary<string, int> rfqCount = new Dictionary<string, int>();
            rfqCount.Add("Total", rfqList.Count());
            rfqCount.Add("SupplierAccepted", rfqList.Sum(r => r.Notification.Count(n => n.SupplierStatus == "Accepted")));
            rfqCount.Add("BuyerAccepted", rfqList.Sum(r => r.Notification.Count(n => n.BuyerStatus == "Accepted")));
            rfqCount.Add("BidsConducted", rfqList.Count(r => r.BiddingStatus == "Completed"));
            rfqCount.Add("BidsAwarded", rfqList.Sum(r => r.Notification.Count(n => n.AwardedStatus == "Awarded")));
            return rfqCount;
        }
        public Dictionary<string, int> GetBuyerProductRfqCount(DashboardViewModel model)
        {
            List<RFQ> rfqList = new List<RFQ>();
            if(model.Month == "All")
            {
                rfqList = rfqs.Find<RFQ>(match => match.BuyerId == model.Id && match.DeletedStatus == 0)
                .ToEnumerable()
                .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("yyyy") == model.Year)
                .ToList();
            }
            else
            {
                rfqList = rfqs.Find<RFQ>(match => match.BuyerId == model.Id && match.DeletedStatus == 0)
                .ToEnumerable()
                .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("yyyy") == model.Year)
                .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("MMMM") == model.Month)
                .ToList();
            }
            List<string> ProductList = new List<string>();
            foreach (var rfq in rfqList)
            {
                List<Notification> notification = new List<Notification>();
                notification = rfq.Notification;
                foreach(var note in notification)
                {
                    ProductList.Add(note.Product.ProductCode);
                }
            }
            List<string> products = ProductList.Distinct().ToList();
            Dictionary<string, int> ProductRfqCount = new Dictionary<string, int>();
            foreach (var code in products)
            {
                int count = 0;
                foreach (var productCode in ProductList)
                {
                    if (productCode == code)
                    {
                        count++;
                    }
                }
                ProductRfqCount.Add(code, count);
            }
            //codes.Add("Product", ProductCodes);
            return ProductRfqCount;
        }

        //Returns the count of Rfqs Buyer created in 12 months
        public Dictionary<string, int> GetBuyerMonthlyRfqData(DashboardViewModel model)
        {
            Dictionary<string, int> monthlyRfqData = new Dictionary<string, int>();
            if(model.Month == "All")
            {
                monthlyRfqData = rfqs.Find(r => r.BuyerId == model.Id && r.DeletedStatus == 0)
                .ToEnumerable()
                .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("yyyy") == model.Year)
                .GroupBy(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("MMMM"))
                .OrderBy(r => DateTime.ParseExact(r.Key, "MMMM", CultureInfo.InvariantCulture))
                .ToDictionary(r => r.Key, r => r.Count());
            }
            else
            {
                monthlyRfqData = rfqs.Find(r => r.BuyerId == model.Id && r.DeletedStatus == 0)
                .ToEnumerable()
                .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("yyyy") == model.Year)
                .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("MMMM") == model.Month)
                .GroupBy(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("MMMM"))
                .OrderBy(r => DateTime.ParseExact(r.Key, "MMMM", CultureInfo.InvariantCulture))
                .ToDictionary(r => r.Key, r => r.Count());
            }

            return monthlyRfqData;
        }

        //Returns the count of Rfqs Supplier Received in 12 months
        public Dictionary<string, int> GetSupplierMonthlyRfqData(DashboardViewModel model)
        {
            Dictionary<string, int> monthlyRfqData = new Dictionary<string, int>();
            if(model.Month == "All")
            {
                monthlyRfqData = rfqs.Find(r => r.Notification.Any(n => n.SupplierId == model.Id))
                    .ToEnumerable()
                    .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("yyyy") == model.Year)
                    .GroupBy(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("MMMM"))
                    .OrderBy(r => DateTime.ParseExact(r.Key, "MMMM", CultureInfo.InvariantCulture))
                    .ToDictionary(r => r.Key, r => r.Count());
            }
            else
            {
                monthlyRfqData = rfqs.Find(r => r.Notification.Any(n => n.SupplierId == model.Id))
                    .ToEnumerable()
                    .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("yyyy") == model.Year)
                    .Where(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("MMMM") == model.Month)
                    .GroupBy(r => DateTime.ParseExact(r.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("MMMM"))
                    .OrderBy(r => DateTime.ParseExact(r.Key, "MMMM", CultureInfo.InvariantCulture))
                    .ToDictionary(r => r.Key, r => r.Count());
            }

            return monthlyRfqData;
        }

        //Returns the average Product Data Rating
        public Dictionary<string, int> GetProductRating(DashboardViewModel model)
        {
            List<Product> productData = new List<Product>();
            if(model.Month == "All")
            {
                productData = products.Find(p => p.SupplierId == model.Id && p.DeletedStatus == 0)
                .ToEnumerable()
                .Where(p => DateTime.ParseExact(p.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("yyyy") == model.Year)
                .ToList();
            }
            else
            {
                productData = products.Find(p => p.SupplierId == model.Id && p.DeletedStatus == 0)
                .ToEnumerable()
                .Where(p => DateTime.ParseExact(p.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("yyyy") == model.Year)
                .Where(p => DateTime.ParseExact(p.CreatedDate, "ddd, dd MMM yyyy HH:mm:ss GMT", CultureInfo.InvariantCulture).ToString("MMMM") == model.Month)
                .ToList();
            }
            
            Dictionary<string, int> productList = new Dictionary<string, int>();
            foreach(var prod in productData)
            {
                int average = 0;
                if(prod.Feedback.Count() != 0)
                {
                    average = (int)prod.Feedback.Average(f => f.Value);
                }
                else
                {
                    average = 0;
                }
                
                //if(prod.Rating.Count()!=0)
                //{
                //    average = (int)prod.Rating.Average();
                //}
                productList.Add(prod.ProductCode, average);
            }
            return productList;
        }

        //Returns the RFQ Rating
        public Dictionary<string, int> GetRFQRating(string id)
        {
            var rfqData = rfqs.Find(r => r.BuyerId == id && r.DeletedStatus == 0).ToList();
            Dictionary<string, int> rfqList = new Dictionary<string, int>();
            foreach (var rfq in rfqData)
            {
                List<Notification> notifications = new List<Notification>();
                notifications = rfq.Notification;
                int a = 0, c=0, avg =0;
                foreach(var note in notifications)
                {
                    if(note.Supplier.RatingValue!=0)
                    {
                        a += note.Supplier.RatingValue;
                        c++;
                    }
                }
                if(c!=0)
                {
                    avg = (int)(a / c);
                }
                rfqList.Add(rfq.RFQId, avg);
            }
            //var rfqList = rfqs.Find(r => r.BuyerId == id && r.DeletedStatus == 0)
            //    .ToEnumerable()
            //    .GroupBy(r => r.RFQId)
            //    .ToDictionary(rfq => rfq.Key, rfq => (int)(rfq.SelectMany(n => n.Notification.Select(n =>n.Supplier.RatingValue)).Average()));
            return rfqList;
 

        }

    }
}
