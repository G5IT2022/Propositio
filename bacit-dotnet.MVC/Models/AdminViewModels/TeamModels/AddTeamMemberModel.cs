using bacit_dotnet.MVC.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bacit_dotnet.MVC.Models.AdminViewModels.TeamModels
{
    /**
    * Klasse/modell legge til bruker til Team (Admin)
    * @Parameter get, set
    * 
    */
    public class AddTeamMemberModel
    {
        public int team_lead_id { get; set; }
        public string team_name { get; set; }
        public List<SelectEmployeesForNewTeamModel> selectEmployeesForNewTeam { get; set; }
        public List<SelectListItem> selectTeamleader { get; set; }

    }
}
