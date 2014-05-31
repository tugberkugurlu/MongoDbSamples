using System.Collections.Generic;

namespace MongoDbSafeUpdateSample.Web.Resources
{
    public class CategoryResource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<ImageResource> Types { get; set; }
    }
}