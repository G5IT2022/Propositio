using Microsoft.AspNetCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using bacit_dotnet.MVC.Entities;


namespace bacit_dotnet.MVC.Models.AdminViewModels.TeamModels
{
    public class AdminEditTeamModel
    {
        [MaxLength(1000, ErrorMessage = "Du kan ikke lage ett navn med over 100 karakterer")]
        [DisplayName("Redigere Teamnavn")]
        [Required(ErrorMessage = "Teamet må ha ett navn")]
        public string team_navn { get; set; }


        [MinLength(50, ErrorMessage = "Beskrivelsen må minst være 50 karakterer")]
        [DisplayName("Team Beskrivelse")]
        [Required(ErrorMessage = "Venligst legg inn en beskrivelse på teamet")]
        public string team_beskrivelse { get; set; }



        public TeamEntity team { get; set; }

    }
}


