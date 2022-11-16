using bacit_dotnet.MVC.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace bacit_dotnet.MVC.Models.Suggestion
{
    public class SuggestionRegisterModel
    {
        [DisplayName("Tittel: ")]
        //[Required(ErrorMessage = "Vennligst legg inn en tittel!")]
        [Required(ErrorMessage = "Vennlist legg inn et nytt ansattnummer.")]
        //[StringLength(200, MinimumLength = 5, ErrorMessage = "Vennligst skriv en ordentlig tittel!")]
        public string title { get; set; }
        [DisplayName("Beskrivelse: ")]
        [Required(ErrorMessage = "Vennligst legg inn en beskrivelse!")]
       // [StringLength(10000, MinimumLength = 10, ErrorMessage = "Vennligst skriv et ordentlig forslag!")]
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
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd.M.yyyy}")]
        public DateTime dueByTimestamp { get; set; }
    }
}