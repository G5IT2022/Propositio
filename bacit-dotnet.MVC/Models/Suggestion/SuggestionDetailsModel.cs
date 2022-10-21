using bacit_dotnet.MVC.Entities;
using System.ComponentModel.DataAnnotations;

namespace bacit_dotnet.MVC.Models.Suggestion
{
    public class SuggestionDetailsModel
    {
        public SuggestionEntity suggestion { get; set; }
        public EmployeeEntity employee { get; set; }        
        public CommentEntity comment { get; set; }

        //Lage ny kommentar
        public string description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime createdTimestamp { get; set; }

    }
}
