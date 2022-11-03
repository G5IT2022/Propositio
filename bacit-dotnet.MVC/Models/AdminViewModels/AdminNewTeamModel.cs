using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Models.AdminViewModels
{
    public class AdminNewTeamModel
    {
        public int team_id { get; set; }
        public string team_name { get; set; }        
        public List<EmployeeEntity> employees { get; set; }  
        public List<EmployeeEntity> addedEmployees { get; set; }
    }
}
