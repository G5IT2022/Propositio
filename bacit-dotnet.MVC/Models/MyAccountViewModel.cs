using bacit_dotnet.MVC.Entities;
namespace bacit_dotnet.MVC.Models
{
    public class MyAccountViewModel
    {

        public List<TeamEntity> teams { get; set; }

        public EmployeeEntity employee { get; set; }
        public List<SuggestionEntity> suggestions { get; set; }
        public List<CategoryEntity> categories { get; set; }
       
    }
}

