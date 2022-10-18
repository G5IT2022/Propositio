using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Models.Suggestion
{
    public class SuggestionDetailsModel
    {
        public SuggestionEntity suggestion { get; set; }
        public EmployeeEntity employee { get; set; }        
        public CommentEntity comment { get; set; }

        //Create new comment
        public string description { get; set; }
        public DateTime dueByTimestamp { get; set; }

    }
}
