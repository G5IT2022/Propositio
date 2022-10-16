using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace bacit_dotnet.MVC.Models
{
  public class AdminNewUserModel
    {
        [DisplayName("Nytt ansattnummer:")]
        [Required(ErrorMessage = "Vennlist legg inn et nytt ansattnummer.")]
        [Range(000, int.MaxValue, ErrorMessage ="Nytt ansattnummer må være minst 3 siffer langt.")]
        public int emp_id { get; set; }
        public int authentication_role_id { get; set; }
        [DisplayName("Fornavn:")]
        [Required(ErrorMessage = "Vennligst legg inn fornavn på den ansatte.")]
        [MaxLength(100, ErrorMessage = "Et nytt navn kan være maks 200 karakterer.")]

        public string first_name { get; set; }
        [DisplayName("Etternavn:")]
        [Required(ErrorMessage = "Vennligst legg inn etternavn på den ansatte.")]
        [MaxLength(100, ErrorMessage = "Et nytt navn kan være maks 200 karakterer.")]

        public string last_name { get; set; }
        [DisplayName("Passord:")]
        [Required(ErrorMessage = "Vennligst legg inn et nytt passord.")]
        [MinLength(10, ErrorMessage ="Et nytt passord må være minst 10 karakterer langt.")]
        [MaxLength(200, ErrorMessage = "Et nytt navn kan være maks 200 karakterer.")]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}
