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
        public DateTime timestamp { get; set; }
        public List<CommentEntity> comments { get; set; }
        public List<SuggestionEntity> suggestions { get; set; }
        public List<EmployeeEntity> employees { get; set; }
    }
}
