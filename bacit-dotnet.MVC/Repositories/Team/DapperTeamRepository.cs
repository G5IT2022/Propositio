﻿using bacit_dotnet.MVC.DataAccess;
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

        public List<EmployeeEntity> Get(string name)
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var query = @"SELECT name, role_id from Employee";
                var employees = connection.Query<EmployeeEntity>(query);

                return employees.ToList();
            }
        }


      
    }
}
