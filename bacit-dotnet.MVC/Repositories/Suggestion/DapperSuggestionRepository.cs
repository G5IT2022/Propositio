using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using MySqlConnector;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace bacit_dotnet.MVC.Repositories.Suggestion
{
    public class DapperSuggestionRepository : ISuggestionRepository
    {
        private readonly ISqlConnector sqlConnector;

        public DapperSuggestionRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }

        public void Add(SuggestionEntity suggestion)
        {
            string suggestionQuery = @"INSERT INTO Suggestion(title, description, status, isJustDoIt, ownership_emp_id, poster_emp_id, timestamp_id)
                VALUES (@title, @description, 'PLAN', @isJustDoIt, @ownership_emp_id, @poster_emp_id, @timestamp_id)";
            string cateogiresQuery = @"INSERT INTO SuggestionCategory(suggestion_id, category_id) VALUES (@suggid, @categoryid)";


            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                connection.Execute(suggestionQuery, new { title = suggestion.title, description = suggestion.description, isJustDoIt = suggestion.isJustDoIt, ownership_emp_id = suggestion.ownership_emp_id, poster_emp_id = suggestion.poster_emp_id, timestamp_id = suggestion.timestamp_id });
                foreach (CategoryEntity category in suggestion.categories)
                { 
                    connection.Execute(cateogiresQuery, new { suggid = suggestion.suggestion_id, categoryid = category.category_id});  
                }
            }
        }

        public int getNewSuggestionID()
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var count = connection.QueryFirst<int>("SELECT COUNT(*) FROM Suggestion");
                return count + 1;
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
