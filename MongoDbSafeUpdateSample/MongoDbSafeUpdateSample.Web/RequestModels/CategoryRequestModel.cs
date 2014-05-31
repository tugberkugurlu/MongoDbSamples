using System.ComponentModel.DataAnnotations;

namespace MongoDbSafeUpdateSample.Web.RequestModels
{
    public class CategoryRequestModel
    {
        [Required]
        public string Name { get; set; }
    }
}