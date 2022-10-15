using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using MySqlConnector;

namespace bacit_dotnet.MVC.Repositories.Comment
{
    public class DapperCommentRepository : ICommentRepository
    {
        private readonly ISqlConnector sqlConnector;
        //Constructor
        public DapperCommentRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;

        }

        public int Create(CommentEntity comment)
        {
            var query = @"INSERT INTO Comment(emp_id, suggestion_id, description) VALUES(@emp_id, @suggestion_id, @description)";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var affectedRows = connection.Execute(query, new { emp_id = comment.emp_id, suggestion_id = comment.suggestion_id, description = comment.description });
                return affectedRows;
            }
        }

    

        public CommentEntity Get(int comment_id)
        {
            var query = @"SELECT * FROM Comment WHERE comment_id = @comment_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var category = connection.QueryFirstOrDefault<CommentEntity>(query, new { comment_id = comment_id });
                return category;
            }
        }

        public List<CommentEntity> GetAll()
        {
           var query = @"SELECT * FROM SuggestionComment";
           using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var comments = connection.Query<CommentEntity>(query);
                return comments.ToList();
            }
        }

        public List<CommentEntity> GetCommentsForSuggestion(int suggestion_id)
        {
            var query = @"SELECT * FROM Comment WHERE suggestion_id = @suggestion_id";
            using(var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var comments = connection.Query<CommentEntity>(query, new {suggestion_id = suggestion_id});
                return comments.ToList();
            }
        }

        public int Update(CommentEntity comment)
        {
            throw new NotImplementedException();
        }
        public int Delete(CommentEntity comment)
        {
            var query = @"DELETE FROM Comment WHERE comment_id = @comment_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var affectedRows = connection.Execute(query, new { comment_id = comment.comment_id });
                return affectedRows;
            }
        }
    }
}
