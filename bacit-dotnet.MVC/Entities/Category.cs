using Dapper.Contrib.Extensions;

namespace bacit_dotnet.MVC.Entities
{
    [Table("Category")]
    public class CategoryEntity
    {
        public int category_id { get; set; }
        public string category_name { get; set; }
    }
}

