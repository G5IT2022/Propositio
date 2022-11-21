using bacit_dotnet.MVC.Entities;
using System.ComponentModel.DataAnnotations;

namespace bacit_dotnet.MVC.Models.Suggestion
{
    /**
    * Klasse/modell for å vise forslag
    * @Parameter get, set
    * @Return Details.cshtml
    */
    public class SuggestionDetailsModel
    {
        public SuggestionEntity suggestion { get; set; }
        public EmployeeEntity employee { get; set; }

        //Lage ny kommentar
        public string description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/mm/yyyy}")]
        public DateTime createdTimestamp { get; set; }

        public List<ImageEntity> image { get; set; }

    }
}
