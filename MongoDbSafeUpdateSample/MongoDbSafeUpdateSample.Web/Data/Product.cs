using System;
using System.Collections.Generic;

namespace MongoDbSafeUpdateSample.Web.Data
{
    public class Product
    {
        public string Id { get; set; }
        public CategoryReference Category { get; set; }
        public string Name { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public IEnumerable<Image> Images { get; set; }
    }
}