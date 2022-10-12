using bacit_dotnet.MVC.Entities;
using System.ComponentModel.DataAnnotations;

namespace bacit_dotnet.MVC.Models
{
    public class AccountViewModel
    {
        [Required(ErrorMessage = "Du glemte å skrive inn navn.")]
        public string name { get; set; }

        [Required(ErrorMessage = "Husk å skrive passord!")]
        public string password { get; set; }
    }
}
