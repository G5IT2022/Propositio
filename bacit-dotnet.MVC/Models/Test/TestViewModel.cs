using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Models.Test
{
    public class TestViewModel
    {
        public int category_id { get; set; }
        public string category_name { get; set; }

        public List<CategoryEntity> categories { get; set; }
    }
}
