using Microsoft.AspNetCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using bacit_dotnet.MVC.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bacit_dotnet.MVC.Models.AdminViewModels.TeamModels
{
    /**
    * Klasse/Modell for å redigere Team (Admin)
    * @Parameter get, set
    * 
    */
    public class AdminEditTeamModel
    {
        public int team_id { get; set; }        
        //public string team_lead { get; set; }
        //public string employee { get; set; }
        [Display(Name = "Teamnavn")]
        public string team_name { get; set; }
        public int team_lead_id { get; set; }
        public TeamEntity team { get; set; }
        public EmployeeEntity teamleader { get; set; }
        public List<SelectListItem> selectListEmployees { get; set; }
        public List<int> selectedMemberTeamIDs { get; set; }

        public SelectList selectListForTeamLeader { get; set; }
    }
}

