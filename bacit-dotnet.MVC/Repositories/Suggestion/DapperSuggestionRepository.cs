using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using MySqlConnector;

namespace bacit_dotnet.MVC.Repositories.Suggestion
{
    public class DapperSuggestionRepository : ISuggestionRepository
    {
        private readonly ISqlConnector sqlConnector;

        public DapperSuggestionRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }

        public List<SuggestionEntity> getAll()
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var suggestions = connection.Query<SuggestionEntity>("SELECT * FROM Suggestion;");
                return suggestions.ToList();
            }
        }
        public List<SuggestionEntity> getByEmployeeID(int id)
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var suggestions = connection.Query<SuggestionEntity>("SELECT * FROM Suggestion WHERE poster_emp_id = @empid;", new { empid = id });
                return suggestions.ToList();
            }
        }

        public SuggestionEntity getById(int id)
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                return connection.QueryFirst<SuggestionEntity>("SELECT * FROM Suggestion WHERE poster_emp_id = @empid;", new { empid = id });
            }
        }

        public List<SuggestionEntity> getByStatus(STATUS status)
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var suggestions = connection.Query<SuggestionEntity>("SELECT * FROM Suggestion WHERE status = @suggstatus;", new { suggstatus = status });
                return suggestions.ToList();
            }
        }

    }
}
