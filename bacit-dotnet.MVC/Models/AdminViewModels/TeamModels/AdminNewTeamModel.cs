using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Models.AdminViewModels.TeamModels
{
    public class AdminNewTeamModel
    {
        public int team_id { get; set; }
        public int team_lead_id { get; set; }
        public string team_name { get; set; }
    }
}
