using Bidding.API.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Services
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> products;
        private readonly IMongoCollection<Category> categories;
        private readonly IMongoCollection<AxetNotification> axetNotification;
        private readonly IMongoCollection<Supplier> suppliers;
        public ProductService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            suppliers = database.GetCollection<Supplier>(settings.SupplierCollectionName);
            products = database.GetCollection<Product>(settings.ProductCollectionName);
            categories = database.GetCollection<Category>(settings.CategoryCollectionName);
            axetNotification = database.GetCollection<AxetNotification>(settings.AxetNotification);
        }

        public List<Product> Get() =>
            products.Find(product => true).ToList();

        //public List<Product> Get(string id) =>
        //    products.Find<Product>(match => match.Id == id).ToList();

        public List<Product> Get(string id)
        {
            IEnumerable<Product> product = products.Find<Product>(match => match.Id == id).ToList();                   //Get the searched product
            return product.ToList();
        }

        

        public List<Product> GetSuppProd(string id)
        {
            IEnumerable<Product> product = products.Find<Product>(match => match.SupplierId == id && match.DeletedStatus == 0).ToList();                   //Get the searched product

            return product.ToList();
        }


        public IEnumerable<Product> GetSupplierProduct(string userid)
        {
            var prodlist = GetSuppProd(userid);
            return prodlist;
        }

        public Product Create(Product product)
        {
            products.InsertOne(product);
            Supplier supplier = suppliers.Find<Supplier>(match => match.SupplierId == product.SupplierId).FirstOrDefault();
            string supplierName = supplier.SupplierName;
            CreateNotification(supplierName + " has added new product", "ProductVarification", "admin");
            return product;
        }

        public void Update(string id, Product product) =>
            products.ReplaceOne(match => match.Id == id, product);

        public void Remove(Product product) =>
            products.DeleteOne(match => match.Id == product.Id);

        public void Remove(string id) =>
            products.DeleteOne(match => match.Id == id);

        public Product UpdateProductStatus(ProductStatus model)
        {
            Product product = products.Find<Product>(match => match.Id == model.ProductId).FirstOrDefault();                        //Here objectId of RFQ is used to get the corresponding RFQ
            product.Status = model.Status;
          
            product.ModifiedDate = model.ModifiedDate;
            product.UpdatedBy = model.UpdatedBy;
            products.ReplaceOne(match => match.Id == model.ProductId, product);
            return product;
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
