using bacit_dotnet.MVC.Entities;
namespace bacit_dotnet.MVC.Models
{
    public class MyAccountViewModel
    {
        public List<TeamEntity> teams { get; set; }

        public List<EmployeeEntity> employees { get; set; }
       
    }
}

