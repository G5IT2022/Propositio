using System.ComponentModel.DataAnnotations;

namespace bacit_dotnet.MVC.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Finn ikke ditt ansattnummer. Venngligst prøv igjen!")]
        public int emp_id { get; set; }
        [Required, DataType(DataType.Password)]
        public string newPassword { get; set; }
        [Required(ErrorMessage = "Passord samvarer ikke."), DataType(DataType.Password)]
        [Compare ("newPassword")]
        public string confirmPassword { get; set; }
    }
}
