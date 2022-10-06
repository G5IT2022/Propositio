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

        public List<CommentEntity> GetAll()
        {
           using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var comments = connection.Query<CommentEntity>("SELECT * FROM SuggestionComment");
                return comments.ToList();
            }
        }
    }
}
