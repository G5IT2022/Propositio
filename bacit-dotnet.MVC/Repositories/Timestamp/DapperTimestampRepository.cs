using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using MySqlConnector;

namespace bacit_dotnet.MVC.Repositories.Timestamp
{
    public class DapperTimestampRepository : ITimestampRepository
    {
        private readonly ISqlConnector sqlConnector;

        public DapperTimestampRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }
        public TimestampEntity Create(int suggestion_id, DateTime dueByTimestamp)
        {
            var query = "INSERT INTO SuggestionTimestamp(suggestion_id, dueByTimestamp) VALUES (@suggid, @dueByTime)";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                return connection.QueryFirstOrDefault<TimestampEntity>(query, new { suggid = suggestion_id , dueByTime = dueByTimestamp });
            }
        }

        public TimestampEntity Get(int suggestion_id)
        {
            var query = "SELECT * FROM SuggestionTimestamp WHERE suggestion_id = @suggid";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
               return connection.QueryFirstOrDefault<TimestampEntity>(query, new {suggid = suggestion_id});
            }
        }
    }
}
