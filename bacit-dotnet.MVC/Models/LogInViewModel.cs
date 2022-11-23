using bacit_dotnet.MVC.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace bacit_dotnet.MVC.Models
{
    public class LogInViewModel
    {
        [DisplayName("Ansattnummer: ")]
        [Required(ErrorMessage = "Du glemte å skrive inn ansattnr.")]
        public int emp_id { get; set; }

        [DisplayName("Passord: ")]
        [Required(ErrorMessage = "Husk å skrive passord!")]
        public string password { get; set; }
    }
}
