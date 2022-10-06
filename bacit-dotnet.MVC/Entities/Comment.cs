namespace bacit_dotnet.MVC.Entities
{
    public class CommentEntity
    {
        public int comment_id { get; set; }
        public int emp_id { get; set; }
        public int suggestion_id { get; set; }
        public string discription { get; set; }
        public DateTime timeStamp { get; set; }
        public List<CommentEntity> comments { get; set; }
        public List<SuggestionEntity> suggestions { get; set; }
        public List<EmployeeEntity> employees { get; set; }
    }
}
