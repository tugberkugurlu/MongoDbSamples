using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDbSafeUpdateSample.Web.Data;
using MongoDbSafeUpdateSample.Web.RequestModels;
using MongoDbSafeUpdateSample.Web.Resources;

namespace MongoDbSafeUpdateSample.Web.Controllers
{
    public class CategoriesController : ApiController
    {
        private static readonly Lazy<MongoCollection<Category>> CategoryCollection = new Lazy<MongoCollection<Category>>(
            () =>
            {
                MongoClient client = new MongoClient("mongodb://localhost");
                MongoServer server = client.GetServer();
                MongoDatabase database = server.GetDatabase("MongoDbSafeUpdateSample");
                MongoCollection<Category> collection = database.GetCollection<Category>("categories");

                return collection;
            });

        public IEnumerable<CategoryResource> GetCategories()
        {
            // NOTE: In a real world example, you would probably skip & take here.
            IEnumerable<Category> categories = CategoryCollection.Value.FindAll().ToList();
            return Mapper.Map<IEnumerable<Category>, IEnumerable<CategoryResource>>(categories);
        }

        public CategoryResource GetCategory(string id)
        {
            Category category = CategoryCollection.Value.FindOne(Query<Category>.EQ(c => c.Id, id));
            if (category == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Mapper.Map<Category, CategoryResource>(category);
        }

        public HttpResponseMessage PostCategory(CategoryRequestModel requestModel)
        {
            throw new NotImplementedException();
        }

        public CategoryResource PutCategory(string id, CategoryRequestModel requestModel)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage DeleteCategory(string id)
        {
            throw new NotImplementedException();
        }
    }
}