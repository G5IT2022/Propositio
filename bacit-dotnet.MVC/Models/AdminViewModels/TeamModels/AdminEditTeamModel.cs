using Microsoft.AspNetCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using bacit_dotnet.MVC.Entities;


namespace bacit_dotnet.MVC.Models.AdminViewModels.TeamModels
{
    public class AdminEditTeamModel
    {
        public TeamEntity team { get; set; }
        public string team_lead { get; set; }
        public string employee { get; set; }
        public string team_name { get; set; }
        public int team_lead_id { get; set; }
    }
}

