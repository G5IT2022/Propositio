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
            var query = @"SELECT COUNT(s.suggestion_id) AS count_sugg FROM Suggestion AS s
INNER JOIN Employee AS e ON s.ownership_emp_id = e.emp_id
INNER JOIN TeamList AS tl ON e.emp_id = tl.emp_id
INNER JOIN Team AS t ON tl.team_id = t.team_id 
GROUP BY t.team_id ORDER BY count_sugg DESC;";
            List<int> list = new List<int>();

            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                    var result = connection.Query<int>(query);
                    return result.ToList();
            }
        }

        public List<TeamEntity> ListTeams()
        {
            var query = @"SELECT t.team_name FROM Team AS t 
INNER JOIN TeamList AS tl ON t.team_id=tl.team_id 
LEFT JOIN Employee AS e ON tl.emp_id=e.emp_id 
LEFT JOIN Suggestion AS s ON e.emp_id=s.ownership_emp_id 
GROUP BY t.team_id ORDER BY COUNT(s.suggestion_id) DESC;";
        

            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                    var result = connection.Query<TeamEntity>(query);
                    return result.ToList();

            }
        }
    }
}
