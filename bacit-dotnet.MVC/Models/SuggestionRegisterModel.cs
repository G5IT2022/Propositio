using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Repositories;

namespace bacit_dotnet.MVC.Models
{
    public class SuggestionRegisterModel
    {
        public string title { get; set; }
        public string description { get; set; }
        public List<CategoryEntity> categories { get; set; }
        public bool isJustDoIt { get; set; }
    }
}