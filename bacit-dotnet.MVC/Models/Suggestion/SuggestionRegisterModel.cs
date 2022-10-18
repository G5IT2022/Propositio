using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace bacit_dotnet.MVC.Models.Suggestion
{
    public class SuggestionRegisterModel
    {
        [Required(ErrorMessage = "Vennligst legg inn en tittel!")]
        [MinLength(10, ErrorMessage = "Vennligst skriv en ordentlig tittel!")]
        [DisplayName("Tittel: ")]
        public string title { get; set; }
        [Required(ErrorMessage = "Vennligst legg inn en beskrivelse!")]
        [MinLength(10, ErrorMessage = "Vennligst skriv et ordentlig forslag!")]
        [DisplayName("Beskrivelse: ")]
        public string description { get; set; }
        [DisplayName("Kategorier: ")]
        public List<CategoryEntity> categories { get; set; }
        [DisplayName("Just Do It?")]
        public bool isJustDoIt { get; set; }

        public DateTime dueByTimestamp { get; set; }
    }
}