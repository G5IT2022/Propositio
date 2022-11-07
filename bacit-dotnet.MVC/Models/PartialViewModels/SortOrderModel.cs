using bacit_dotnet.MVC.Entities;
using System.ComponentModel.DataAnnotations;

namespace bacit_dotnet.MVC.Models
{
    public class SortOrderModel
    {
        public string controllername { get; set; }
        public  string actionName { get; set; }
        public List<CategoryEntity> categories { get; set; }
        
    }
}