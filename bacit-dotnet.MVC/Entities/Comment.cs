using Dapper.Contrib.Extensions;

namespace bacit_dotnet.MVC.Entities
{
    [Table ("SuggestionComment")]
    public class CommentEntity
    {
        public int comment_id { get; set; }
        public int emp_id { get; set; }
        public int suggestion_id { get; set; }
        public string description { get; set; }
        public DateTime createdTimestamp { get; set; }
        public DateTime lastUpdatedTimestamp { get; set; }
        public SuggestionEntity suggestion { get; set; }
        public EmployeeEntity poster { get; set; }
    }
}
