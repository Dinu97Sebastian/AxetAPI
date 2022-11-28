using Bidding.API.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Services
{
    public class BuyerService
    {
        private readonly IMongoCollection<Buyer> buyers;
        private readonly IMongoCollection<Sequence> sequence;
        private readonly IMongoCollection<User> users;
        private readonly IMongoCollection<AxetNotification> axetNotification;

        public BuyerService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            buyers = database.GetCollection<Buyer>(settings.BuyerCollectionName);
            sequence = database.GetCollection<Sequence>(typeof(Sequence).Name);
            users = database.GetCollection<User>(settings.UserCollectionName);
            axetNotification = database.GetCollection<AxetNotification>(settings.AxetNotification);
        }

        public IEnumerable<Buyer> Get()
        {

            var user = from o in buyers.Find(buyer => true).ToList()

                       select new Buyer
                       {
                           Id = o.Id,
                           BuyerId = o.BuyerId,
                           BuyerName = o.BuyerName,
                           CompanyName=o.CompanyName
                       };
            return user.ToList();

        }
        public List<Buyer> Get(string id)
        {
            IEnumerable<Buyer> buyer = buyers.Find<Buyer>(match => match.BuyerId == id).ToList();
            return buyer.ToList();
        }

        public Buyer Create(Buyer buyer)
        {
            buyers.InsertOne(buyer);
            CreateNotification(buyer.BuyerName + " " + "waiting for your approval.", "UserList","admin");
            return buyer;
        }

        public int GetSequenceValue(string sequenceName)
        {
            var filter = Builders<Sequence>.Filter.Eq(s => s.SequenceName, sequenceName);
            var update = Builders<Sequence>.Update.Inc(s => s.SequenceValue, 1);

            var result = sequence.FindOneAndUpdate(filter, update, new FindOneAndUpdateOptions<Sequence, Sequence> { IsUpsert = true, ReturnDocument = ReturnDocument.After });

            return result.SequenceValue;
        }

        public void Update(string id, Buyer buyer)
        {
            if (buyer.DeletedStatus == 1)
            {
                var user = users.Find<User>(match => match.UserID == buyer.BuyerId).FirstOrDefault();
                user.Status = "InActive";
                users.ReplaceOne(match => match.UserID == buyer.BuyerId, user);
            }
            buyers.ReplaceOne(match => match.BuyerId == id, buyer);
        }

        public void Remove(Buyer buyer) =>
            buyers.DeleteOne(match => match.Id == buyer.Id);

        public void Remove(string id) =>
            buyers.DeleteOne(match => match.Id == id);

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
