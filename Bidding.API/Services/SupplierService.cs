using Bidding.API.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Services
{
    public class SupplierService
    {
        private readonly IMongoCollection<Supplier> suppliers;
        private readonly IMongoCollection<Sequence> sequence;
        private readonly IMongoCollection<Country> countries;
        private readonly IMongoCollection<State> states;
        private readonly IMongoCollection<City> cities;
        private readonly IMongoCollection<User> users;
        private readonly IMongoCollection<AxetNotification> axetNotification;
        public SupplierService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            suppliers = database.GetCollection<Supplier>(settings.SupplierCollectionName);
            countries = database.GetCollection<Country>(settings.CountryCollectionName);
            states = database.GetCollection<State>(settings.StateCollectionName);
            cities = database.GetCollection<City>(settings.CityCollectionName);
            sequence = database.GetCollection<Sequence>(typeof(Sequence).Name);
            users = database.GetCollection<User>(settings.UserCollectionName);
            axetNotification = database.GetCollection<AxetNotification>(settings.AxetNotification);
        }

        public List<Supplier> Get()
        {
            var suppliersData = from s in suppliers.Find(supplier => true).ToList()
                                select new Supplier
                                {
                                    Id=s.Id,
                                    SupplierId = s.SupplierId,
                                    CompanyName = s.CompanyName,
                                    SupplierName=s.SupplierName,
                                };
            return suppliersData.ToList();
        }
           

        //public Supplier Get(string id)
        //{
        //    Supplier supplier = suppliers.Find<Supplier>(match => match.Id == id).FirstOrDefault();
        //    return supplier;
        //}


        public List<Supplier> Get(string id)
        {
            IEnumerable<Supplier> supplier = suppliers.Find<Supplier>(match => match.SupplierId == id).ToList();
            return supplier.ToList();
        }

        public Supplier Create(Supplier supplier)
        {
            suppliers.InsertOne(supplier);
            CreateNotification(supplier.SupplierName + " " + "waiting for your approval.", "UsersList", "admin");
            return supplier;
        }

        public int GetSequenceValue(string sequenceName)
        {
            var filter = Builders<Sequence>.Filter.Eq(s => s.SequenceName, sequenceName);
            var update = Builders<Sequence>.Update.Inc(s => s.SequenceValue, 1);

            var result = sequence.FindOneAndUpdate(filter, update, new FindOneAndUpdateOptions<Sequence, Sequence> { IsUpsert = true, ReturnDocument = ReturnDocument.After });

            return result.SequenceValue;
        }

        public void Update(string id, Supplier supplier)
        {
            if(supplier.DeletedStatus == 1)
            {
                var user = users.Find<User>(match => match.UserID == supplier.SupplierId).FirstOrDefault();
                user.Status = "InActive";
                users.ReplaceOne(match => match.UserID == supplier.SupplierId, user);
            }
            suppliers.ReplaceOne(match => match.SupplierId == id, supplier);
        }
            

        public void Remove(Supplier supplier) =>
            suppliers.DeleteOne(match => match.Id == supplier.Id);

        public void Remove(string id) =>
            suppliers.DeleteOne(match => match.Id == id);

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
