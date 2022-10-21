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

        public void Create(SuggestionEntity suggestion)
        {
            string suggestionQuery = @"INSERT INTO Suggestion(title, description, status, ownership_emp_id, author_emp_id)
                VALUES (@title, @description, 'PLAN', @ownership_emp_id, @poster_emp_id)";
            string cateogiresQuery = @"INSERT INTO SuggestionCategory(suggestion_id, category_id) VALUES (@suggid, @categoryid)";


            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                connection.Execute(suggestionQuery, new { title = suggestion.title, description = suggestion.description, ownership_emp_id = suggestion.ownership_emp_id, poster_emp_id = suggestion.author_emp_id });
                foreach (CategoryEntity category in suggestion.categories)
                {
                    connection.Execute(cateogiresQuery, new { suggid = suggestion.suggestion_id, categoryid = category.category_id });
                }
            }
        }

        public void Favorite(int id, bool update)
        {
            var updateQuery = @"UPDATE Suggestion SET favorite = @state WHERE suggestion_id = @suggestion_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                connection.Execute(updateQuery, new { state = update, suggestion_id = id });
            }

        }

        public int GetNewSuggestionID()
        {
            var query = @"SELECT COUNT(*) FROM Suggestion";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var count = connection.QueryFirst<int>(query);
                return count + 1;
            }
        }

        public List<SuggestionEntity> GetAll()
        {
            var query = @"SELECT * FROM Suggestion";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var suggestions = connection.Query<SuggestionEntity>(query);
                return suggestions.ToList();
            }
        }
        public List<SuggestionEntity> GetByEmployeeID(int author_emp_id)
        {
            var query = @"SELECT * FROM Suggestion WHERE author_emp_id = @author_emp_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var suggestions = connection.Query<SuggestionEntity>(query, new { author_emp_id = author_emp_id });
                return suggestions.ToList();
            }
        }

        public SuggestionEntity GetById(int id)
        {
            var query = @"SELECT * FROM Suggestion WHERE suggestion_id = @suggestion_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                return connection.QueryFirstOrDefault<SuggestionEntity>(query, new { suggestion_id = id });
            }
        }

        public List<SuggestionEntity> GetByStatus(STATUS status)
        {
            var query = @"SELECT * FROM Suggestion WHERE status = status";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var suggestions = connection.Query<SuggestionEntity>(query, new { status = status });
                return suggestions.ToList();
            }
        }
    }

}


