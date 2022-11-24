using bacit_dotnet.MVC.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace bacit_dotnet.MVC.Models.Suggestion
{
    /**
    * Klasse/Modell for å registrere ett forslag
    * @Parameter get, set
    * 
    */
    public class SuggestionRegisterModel
    {
        [DisplayName("Tittel: ")]
        [Required(ErrorMessage = "Venligst legg inn en tittel!")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Vennligst skriv en ordentlig tittel!")]
        public string title { get; set; }
        [DisplayName("Beskrivelse: ")]
        [Required(ErrorMessage = "Vennligst legg inn en beskrivelse!")]
        [StringLength(10000, MinimumLength = 10, ErrorMessage = "Forslaget må ha minst 10 karakterer!")]
        public string description { get; set; }
        [DisplayName("Kategorier: ")]
        public List<CategoryEntity> categories { get; set; }
        [DisplayName("Just Do It?")]
        public bool isJustDoIt { get; set; }

        public int ownership_emp_id { get; set; }
        public List<SelectListItem> possibleResponsibleEmployees { get; set; }

        [DisplayName("Frist:")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime dueByTimestamp { get; set; }
    }
}