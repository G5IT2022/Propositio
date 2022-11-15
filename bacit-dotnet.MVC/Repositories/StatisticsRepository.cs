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
        /**
         * Metode som henter ut tall for antall forslag team er ansvarlig for
         * Returnerer en liste med tall
         */
        public List<int> ListNumberOfSuggestionsPerTeam()
        {
            //spørring
            var query = @"SELECT COUNT(s.suggestion_id) FROM Suggestion AS s 
INNER JOIN Employee AS e ON s.ownership_emp_id=e.emp_id 
INNER JOIN TeamList AS tl ON e.emp_id=tl.emp_id 
RIGHT JOIN Team AS t ON t.team_id=tl.team_id 
GROUP BY t.team_id ORDER BY COUNT(s.suggestion_id) DESC;";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                    var numbers = connection.Query<int>(query);
                    return numbers.ToList();
            }
        }

        /**
         * Metode som henter tall for antall forslag som er i hver status => 'PLAN', 'DO', 'STUDY', 'ACT', 'ISJUSTDOIT'
         * Returnerer en liste med tall
         */
        public List<int> ListNumberOfSuggestionsPerStatus()
        {
            //spørring
            var query = @"SELECT COUNT(s.suggestion_id) AS count_sugg FROM Suggestion AS s 
GROUP BY STATUS ORDER BY count_sugg;";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                var numbers = connection.Query<int>(query);
                return numbers.ToList();
            }
        }
        /**
         * Metode som henter tall for antall forslag for de topp 5 ansatte med flest forslag
         * Returnerer en liste med tall
         */
        public List<int> ListTopNumberOfSuggestionsOfTopFiveEmployees()
        {
            //spørring
            var query = @"SELECT COUNT(s.suggestion_id) AS count_sugg FROM Suggestion AS s 
INNER JOIN Employee AS e ON s.author_emp_id=e.emp_id GROUP BY e.emp_id ORDER BY count_sugg DESC LIMIT 5;";

            //kobler til databasen
            using(var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                var result = connection.Query<int>(query);
                return result.ToList();
            }
        }

        /**
         * Metode som henter en liste med navn på teams
         * Returnerer en liste med team_name
         */
        public List<TeamEntity> ListTeams()
        {
            //spørring
            var query = @"SELECT t.team_name FROM Suggestion AS s 
INNER JOIN Employee AS e ON s.ownership_emp_id=e.emp_id 
INNER JOIN TeamList AS tl ON e.emp_id=tl.emp_id 
RIGHT JOIN Team AS t ON t.team_id=tl.team_id 
GROUP BY t.team_id ORDER BY COUNT(s.suggestion_id) DESC;";
        
            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                    var result = connection.Query<TeamEntity>(query);
                    return result.ToList();

            }
        }
        /**
         * Metode som henter status
         * returnerer liste med status
         */
        public List<SuggestionEntity> ListStatuses()
        {
            //spørring
            var query = @"SELECT s.status FROM Suggestion AS s 
GROUP BY STATUS ORDER BY COUNT(s.suggestion_id);";

            //kobler til databasen
            using(var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                var statuses = connection.Query<SuggestionEntity>(query);
                return statuses.ToList();
            }
        }

        /**
         * Metode som henter navn til topp 5 ansatte med flest forslag
         * Returnerer en liste med navn
         */
        public List<EmployeeEntity> ListEmployees()
        {
            //spørring
            var query = @"SELECT e.name FROM Employee AS e 
INNER JOIN Suggestion AS s ON s.author_emp_id=e.emp_id GROUP BY e.emp_id ORDER BY COUNT(s.suggestion_id) DESC LIMIT 5;";

            //kobler til databasen
            using(var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                var result = connection.Query<EmployeeEntity>(query);
                return result.ToList();
            }
        }
    }
}
