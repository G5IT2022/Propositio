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

        public void Add(SuggestionEntity entity)
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                string sql = @"INSERT INTO Suggestion(title, description, status, isJustDoIt, ownership_emp_id, poster_emp_id, timestamp_id) VALUES (@title, @description, 'PLAN', @isJustDoIt, @ownership_emp_id, @poster_emp_id, @timestamp_id)";
                connection.Execute(sql, new
                {
                   title = entity.title,
                   description = entity.description,
                   status =  entity.status,
                   isJustDoIt =  entity.isJustDoIt,
                   ownership_emp_id =  entity.ownership_emp_id,
                   poster_emp_id = entity.poster_emp_id,
                   timestamp_id =  entity.timestamp_id
                });
            }
        }

        public int getLatestSuggestionID()
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var suggestion = connection.QueryFirst<SuggestionEntity>("SELECT * FROM Suggestion ORDER BY suggestion_id DESC LIMIT 1");
                return suggestion.suggestion_id;
            }
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
