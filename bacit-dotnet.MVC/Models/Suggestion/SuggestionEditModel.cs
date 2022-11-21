using bacit_dotnet.MVC.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bacit_dotnet.MVC.Models.Suggestion
{
    /**
    * Klasse/Modell for å redigere forslag
    * @Parameter get, set
    * @Return
    */
    public class SuggestionEditModel
    {
        public SuggestionEntity suggestion { get; set; }
        public List<SelectListItem> possibleResponsibleEmployees { get; set; }
        public int responsibleEmployeeID { get; set; }
        public string newDescription { get; set; }
        public DateTime newDueByDate { get; set; }
    }
}
