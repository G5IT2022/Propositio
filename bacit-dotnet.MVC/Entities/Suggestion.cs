using Dapper.Contrib.Extensions;

namespace bacit_dotnet.MVC.Entities
{

    public enum STATUS { PLAN, DO, STUDY, ACT, FINISHED, JUSTDOIT}

    [Table ("Suggestion")]
   public class SuggestionEntity
    {
        public int  suggestion_id { get; set; }
        public int ownership_emp_id { get; set; }
        public int author_emp_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public STATUS status { get; set; }
        public bool favorite { get; set; }
        public TimestampEntity timestamp { get; set; }
        public List<CommentEntity> comments { get; set; }
        public List<CategoryEntity> categories { get; set; }
        public List<ImageEntity> images { get; set; }
        public EmployeeEntity author { get; set; }
        public EmployeeEntity responsible_employee { get; set; }

        public SuggestionEntity()
        {
            this.comments = new List<CommentEntity>();
            this.categories = new List<CategoryEntity>();
            this.images = new List<ImageEntity>();
        }

    }
}
