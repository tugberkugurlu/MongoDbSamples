using System;
using System.Collections.Generic;
using System.Web.Http;
using MongoDbSafeUpdateSample.Web.Resources;

namespace MongoDbSafeUpdateSample.Web.Controllers
{
    public class CategoriesController : ApiController
    {
        public IEnumerable<CategoryResource> GetCategories()
        {
            throw new NotImplementedException();
        }

        public CategoryResource GetCategory(string id)
        {
            throw new NotImplementedException();
        }

        public void PostCategory()
        {
            throw new NotImplementedException();
        }
    }
}