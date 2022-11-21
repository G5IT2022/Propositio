using bacit_dotnet.MVC.Entities;
using System.ComponentModel.DataAnnotations;

namespace bacit_dotnet.MVC.Models
{
    public class LogInViewModel
    {
        [Required(ErrorMessage = "Du glemte å skrive inn ansattnr.")]
        public int emp_id { get; set; }

        [Required(ErrorMessage = "Husk å skrive passord!")]
        public string password { get; set; }
    }
}
