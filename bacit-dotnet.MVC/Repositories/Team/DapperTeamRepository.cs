using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using MySqlConnector;
using System.Xml.Linq;

namespace bacit_dotnet.MVC.Repositories.Team
{

    public class DapperTeamRepository : ITeamRepository
    {

        private readonly ISqlConnector sqlConnector;

        public DapperTeamRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }
        public List<TeamEntity> Get(int id)
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var query = @"SELECT t.team_id, t.team_name FROM Team as t INNER JOIN
                TeamList as tl on t.team_id = tl.team_id INNER JOIN Employee as e on e.emp_id = tl.emp_id WHERE e.emp_id = @empid";
                var teams = connection.Query<TeamEntity>(query, new { empid = id });

                return teams.ToList();
            }



        }
        public List<TeamEntity> GetAll()
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var query = @"SELECT team_id, team_name, team_lead_id FROM Team";
                var teams = connection.Query<TeamEntity>(query);

                return teams.ToList();
            }
        }

        public List<TeamEntity> GetAll()
        {
            var query = @"SELECT * FROM Team";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var teams = connection.Query<TeamEntity>(query);
                return teams.ToList();
            }
        }

        public List<EmployeeEntity> GetEmployeesForTeam(int team_id)
        {
            var query = @"SELECT e.emp_id, e.name, e.role_id FROM Employee as e INNER JOIN 
            TeamList as tl ON e.emp_id = tl.emp_id INNER JOIN 
            Team as t on t.team_id = tl.team_id WHERE t.team_id = @team_id";

            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var employees = connection.Query<EmployeeEntity>(query, new {team_id = team_id});
                return employees.ToList();
            }
        }
    }
}
