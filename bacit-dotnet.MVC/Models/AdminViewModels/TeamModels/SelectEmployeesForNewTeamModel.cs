using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Models.AdminViewModels.TeamModels
{
    /**
    * Klasse/Modell for å legge til bruker til nytt Team
    * @Parameter get, set
    * 
    */
    public class SelectEmployeesForNewTeamModel
    {
        public int emp_id { get; set; }
        public string name { get; set; }
        public EmployeeEntity employee { get; set; }
        public bool selected { get; set; }

    }
}
