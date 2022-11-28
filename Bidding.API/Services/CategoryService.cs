using Bidding.API.Models;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Services
{
    public class CategoryService
    {
        private readonly IMongoCollection<Category> categories;

        public CategoryService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            categories = database.GetCollection<Category>(settings.CategoryCollectionName);
        }

        //To list all categories
        public List<Category> Get() =>
            categories.Find(category => true).ToList();

        //To list details of a particular category
        public Category Get(string id)
        {
            var category = categories.Find<Category>(match => match.Id == id).FirstOrDefault();
            return category;
        }
        //To get Subcategory
        public IEnumerable<Subcategory> GetSubcategory(string categoryId)
        {
            var category = Get(categoryId);
            return category?.Subcategories;
        }

        //To get Types
        public IEnumerable<Models.Type> GetTypes(string categoryId, string subcategoryId)
        {
            var subcategory = GetSubcategory(categoryId);
            //IEnumerable<Models.Type> type = new List<Models.Type>();
            var sbCat = subcategory.FirstOrDefault(s => s.Id == subcategoryId);
            return sbCat?.Types;
        }

        //To get attributes
        public Dictionary<string, List<Models.Attribute>> GetAttributes(string categoryId, string subcategoryId, string typeId)
        {
            var types = GetTypes(categoryId, subcategoryId);
            IEnumerable<Models.Attribute> attributes = new List<Models.Attribute>();
            var typ = types.FirstOrDefault(t => t.Id == typeId);
            attributes = typ?.Attributes;
            IEnumerable<Models.Attribute> required = new List<Models.Attribute>();
            required = attributes.Where(a => a.Optional == false).ToList();
            IEnumerable<Models.Attribute> optional = new List<Models.Attribute>();                                                                                               
            optional = attributes.Where(a => a.Optional == true).ToList();
            IEnumerable<Models.Attribute> documents = new List<Models.Attribute>();
            documents = attributes.Where(a => a.Document == true).ToList();
            Dictionary<string, List<Models.Attribute>> attr = new Dictionary<string, List<Models.Attribute>>();
            attr.Add("required", required.ToList());
            attr.Add("optional", optional.ToList());
            attr.Add("documents", documents.ToList());
            
            //var text = (required ?? Enumerable.Empty<Models.Attribute>()).Concat(optional ?? Enumerable.Empty<Models.Attribute>()); //amending `<string>` to the appropriate type
            return attr;
        }

        //To get rating
        public Dictionary<string, List<RatingField>> GetRating(string categoryId, string subcategoryId, string typeId)
        {
            var typedata = categories.Find(c => c.Name == categoryId).ToEnumerable()
                .SelectMany(c => c.Subcategories)
                .FirstOrDefault(s => s.Name == subcategoryId)
                .Types.Where(t => t.Name == typeId)
                .FirstOrDefault();

            List<RatingField> ratings = new List<RatingField>();
            ratings = typedata?.RatingFields;
            Dictionary<string, List<RatingField>> ratingFields = new Dictionary<string, List<RatingField>>();
            ratingFields.Add("ratings", ratings.ToList());
            return ratingFields;
        }

        public Category Create(Category category)
        {
            categories.InsertOne(category);
            return category;
        }

        public Category Edit(Category model)
        {
            Category category = categories.Find<Category>(match => match.Id == model.Id).FirstOrDefault();
            category = model;
            categories.ReplaceOne(match => match.Id == model.Id, category);
            return category;
        }

        public void Update(string id, Category category) =>
            categories.ReplaceOne(match => match.Id == id, category);

        public void Remove(Category category) =>
            categories.DeleteOne(match => match.Id == category.Id);

        public void Remove(string id) =>
            categories.DeleteOne(match => match.Id == id);
    }
}
