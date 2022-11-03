using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using MySqlConnector;

namespace bacit_dotnet.MVC.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {

        private readonly ISqlConnector sqlConnector;

        public EmployeeRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }
        public int CreateEmployee(EmployeeEntity emp)
        {
            var query = @"INSERT INTO Employee(emp_id, name, passwordhash,salt, role_id, authorization_role_id) VALUES (@emp_id, @name, @passwordhash, @salt, @role_id, @authorization_role_id)";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                int result = connection.Execute(query, new { emp.emp_id, emp.name, emp.passwordhash, emp.salt, emp.role_id, emp.authorization_role_id });
                return result;
            }
        }

        //Henter en enkelt ansatt basert på employeeid, returnerer en EmployeeEntity med en liste over team de er med i og rolle. 
        /**
         1. Hent ansattnr, navn, role_id på den ansatte
         2. Hent role_name på den ansatte 
         3. Hent team_id, team_navn og team_lead_id på teamene den ansatte er med i. 
         */
        public EmployeeEntity GetEmployee(int emp_id)
        {
            var query = @"SELECT e.emp_id, e.name, e.role_id, r.role_id, r.role_name, t.team_id, t.team_name, t.team_lead_id FROM Employee AS e 
            INNER JOIN Role AS r ON e.role_id = r.role_id INNER JOIN TeamList AS tl ON tl.emp_id = e.emp_id 
            INNER JOIN Team AS t ON tl.team_id = t.team_id WHERE e.emp_id = @emp_id";

            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var emp = connection.Query<EmployeeEntity, RoleEntity, TeamEntity, EmployeeEntity>(query, (employee, role, team) =>
                {
                    employee.role = role;
                    if (employee.teams == null)
                    {
                        employee.teams = new List<TeamEntity>();
                    }
                    employee.teams.Add(team);
                    return employee;
                }, new { emp_id }, splitOn: "role_id, team_id");
                var result = emp.GroupBy(e => e.emp_id).Select(employee =>
                {
                    var groupedEmployee = employee.First();
                    groupedEmployee.teams = employee.Select(e => e.teams.Single()).ToList();
                    return groupedEmployee;
                });
                return result.ElementAt(0);
            }
        }

       

        public List<EmployeeEntity> GetEmployees()
        {
            var query = @"SELECT e.emp_id, e.name, e.role_id, r.role_id, r.role_name, t.team_id, t.team_name, t.team_lead_id FROM Employee AS e 
            INNER JOIN Role AS r ON e.role_id = r.role_id INNER JOIN TeamList AS tl ON tl.emp_id = e.emp_id 
            INNER JOIN Team AS t ON tl.team_id = t.team_id";

            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var emp = connection.Query<EmployeeEntity, RoleEntity, TeamEntity, EmployeeEntity>(query, (employee, role, team) =>
                {
                    employee.role = role;
                    if (employee.teams == null)
                    {
                        employee.teams = new List<TeamEntity>();
                    }
                    employee.teams.Add(team);
                    return employee;
                }, splitOn: "role_id, team_id");
                var result = emp.GroupBy(e => e.emp_id).Select(employee =>
                {
                    var groupedEmployee = employee.First();
                    groupedEmployee.teams = employee.Select(e => e.teams.Single()).ToList();
                    return groupedEmployee;
                });


                return result.ToList();
            }
        }

        public List<SelectListItem> GetEmployeeSelectList()
        {
            var query = @"SELECT emp_id, name FROM Employee";
            List<SelectListItem> list = new List<SelectListItem>();
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = query;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var item = new SelectListItem();
                    item.Value = reader[0].ToString();
                    item.Text = reader[1].ToString();
                    list.Add(item);
                }
                connection.Close();
                return list;
            }
        }

        public TeamEntity GetTeam(int team_id)
        {
            var query = @"SELECT t.team_id, t.team_name, t.team_lead_id, e.emp_id, e.name, r.role_id, r.role_name FROM Team AS t INNER JOIN 
            TeamList AS tl ON t.team_id = tl.team_id INNER JOIN Employee AS e ON e.emp_id = tl.emp_id INNER JOIN Role AS r ON
            e.role_id = r.role_id WHERE t.team_id = @team_id";

            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var team = connection.Query<TeamEntity, EmployeeEntity, RoleEntity, TeamEntity>(query, (team, employee, role) =>
                {
                    if (team.employees == null)
                    {
                        team.employees = new List<EmployeeEntity>();
                    }
                    employee.role = role;
                    team.employees.Add(employee);
                    return team;

                }, new { team_id }, splitOn: "emp_id, role_id");

                var result = team.GroupBy(e => e.team_id).Select(team =>
                {
                    var groupedTeams = team.First();
                    groupedTeams.employees = team.Select(e => e.employees.Single()).ToList();
                    return groupedTeams;
                });


                return result.ElementAt(0);
            }
        }

        public List<TeamEntity> GetTeams()
        {
            var query = @"SELECT t.team_id, t.team_name, t.team_lead_id, e.emp_id, e.name, r.role_id, r.role_name FROM Team AS t INNER JOIN 
            TeamList AS tl ON t.team_id = tl.team_id INNER JOIN Employee AS e ON e.emp_id = tl.emp_id INNER JOIN Role AS r ON
            e.role_id = r.role_id";

            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var team = connection.Query<TeamEntity, EmployeeEntity, RoleEntity, TeamEntity>(query, (team, employee, role) =>
                {
                    if (team.employees == null)
                    {
                        team.employees = new List<EmployeeEntity>();
                    }
                    employee.role = role;
                    team.employees.Add(employee);
                    return team;

                }, splitOn: "emp_id, role_id");

                var result = team.GroupBy(e => e.team_id).Select(team =>
                {
                    var groupedTeams = team.First();
                    groupedTeams.employees = team.Select(e => e.employees.Single()).ToList();
                    return groupedTeams;
                });
                return result.ToList();
            }
        }
    }
}