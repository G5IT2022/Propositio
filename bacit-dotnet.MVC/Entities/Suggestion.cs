using Dapper.Contrib.Extensions;

namespace bacit_dotnet.MVC.Entities
{

    public enum STATUS { PLAN, DO, STUDY, ACT, FINISHED, JUSTDOIT}

    [Table ("Suggestion")]
   public class SuggestionEntity
    {
        public int  suggestion_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public STATUS status { get; set; }
        public bool isFavorite { get; set; }
        public int ownership_emp_id { get; set; }
        public int author_emp_id   { get; set; }
        public TimestampEntity timestamp { get; set; }
      
        
        public List<CommentEntity> comments;
        public List<CategoryEntity> categories;

    }
}
