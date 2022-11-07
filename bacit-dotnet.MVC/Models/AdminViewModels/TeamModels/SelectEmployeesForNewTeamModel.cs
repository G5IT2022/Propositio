using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Models.AdminViewModels.TeamModels
{
    public class SelectEmployeesForNewTeamModel
    {
        public int emp_id { get; set; }
        public string name { get; set; }
        public EmployeeEntity employee { get; set; }
        public bool selected { get; set; }

    }
}
