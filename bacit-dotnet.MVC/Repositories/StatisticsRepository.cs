using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using bacit_dotnet.MVC.Models.Statistics;
using MySqlConnector;
using System.Collections.Generic;

namespace bacit_dotnet.MVC.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly ISqlConnector sqlConnector;
        
        public StatisticsRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }
        public List<int> ListNumberOfSuggestionsPerTeam()
        {
            var query = @"SELECT COUNT(s.suggestion_id) FROM Suggestion AS s 
INNER JOIN Employee AS e ON s.ownership_emp_id=e.emp_id 
INNER JOIN TeamList AS tl ON e.emp_id=tl.emp_id 
RIGHT JOIN Team AS t ON t.team_id=tl.team_id 
GROUP BY t.team_id ORDER BY COUNT(s.suggestion_id) DESC;";

            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                    var result = connection.Query<int>(query);
                    return result.ToList();
            }
        }
        public List<int> ListNumberOfSuggestionsPerStatus()
        {
            var query = @"SELECT COUNT(s.suggestion_id) AS count_sugg FROM Suggestion AS s 
GROUP BY STATUS ORDER BY count_sugg;";

            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var results = connection.Query<int>(query);
                return results.ToList();
            }
        }
        public List<int> ListTopNumberOfSuggestionsOfTopThreeEmployees()
        {
            var query = @"SELECT COUNT(s.suggestion_id) AS count_sugg FROM Suggestion AS s 
INNER JOIN Employee AS e ON s.author_emp_id=e.emp_id GROUP BY e.emp_id ORDER BY count_sugg DESC LIMIT 5;";

            using(var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var result = connection.Query<int>(query);
                return result.ToList();
            }
        }

        public List<TeamEntity> ListTeams()
        {
            var query = @"SELECT t.team_name FROM Suggestion AS s 
INNER JOIN Employee AS e ON s.ownership_emp_id=e.emp_id 
INNER JOIN TeamList AS tl ON e.emp_id=tl.emp_id 
RIGHT JOIN Team AS t ON t.team_id=tl.team_id 
GROUP BY t.team_id ORDER BY COUNT(s.suggestion_id) DESC;";
        

            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                    var result = connection.Query<TeamEntity>(query);
                    return result.ToList();

            }
        }
        public List<SuggestionEntity> ListStatuses()
        {
            var query = @"SELECT s.status FROM Suggestion AS s 
GROUP BY STATUS ORDER BY COUNT(s.suggestion_id);";

            using(var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var statuses = connection.Query<SuggestionEntity>(query);
                return statuses.ToList();
            }
        }
        public List<EmployeeEntity> ListEmployees()
        {
            var query = @"SELECT e.name FROM Employee AS e 
INNER JOIN Suggestion AS s ON s.author_emp_id=e.emp_id GROUP BY e.emp_id ORDER BY COUNT(s.suggestion_id) DESC LIMIT 5;";

            using(var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var result = connection.Query<EmployeeEntity>(query);
                return result.ToList();
            }
        }
    }
}
