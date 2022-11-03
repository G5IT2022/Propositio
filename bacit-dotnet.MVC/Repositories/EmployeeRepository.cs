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
        /**
         * Metode for å registrere ansatte 
         */
        public int CreateEmployee(EmployeeEntity emp)
        {
            //spørring
            var query = @"INSERT INTO Employee(emp_id, name, passwordhash,salt, role_id, authorization_role_id) VALUES (@emp_id, @name, @passwordhash, @salt, @role_id, @authorization_role_id)";

            //kobler til databasen
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
            //spørring
            var query = @"SELECT e.emp_id, e.name, e.role_id, r.role_id, r.role_name, t.team_id, t.team_name, t.team_lead_id FROM Employee AS e 
            INNER JOIN Role AS r ON e.role_id = r.role_id INNER JOIN TeamList AS tl ON tl.emp_id = e.emp_id 
            INNER JOIN Team AS t ON tl.team_id = t.team_id WHERE e.emp_id = @emp_id";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen 
                var emp = connection.Query<EmployeeEntity, RoleEntity, TeamEntity, EmployeeEntity>(query, (employee, role, team) =>
                {
                    //variabel role i koden kobles til variabel role i databasen
                    employee.role = role;
                    if (employee.teams == null)
                    {
                        employee.teams = new List<TeamEntity>();
                    }
                    //legg til team
                    employee.teams.Add(team);
                    //returner ansatt
                    return employee;
                }, new { emp_id }, splitOn: "role_id, team_id");
                var result = emp.GroupBy(e => e.emp_id).Select(employee =>
                {
                    //grupper teamene til den ansatte, dette må gjøres fordi siden vi har mange til mange forhold kobler returnerer den flere rader med data
                    var groupedEmployee = employee.First();
                    groupedEmployee.teams = employee.Select(e => e.teams.Single()).ToList();
                    return groupedEmployee;
                });
                //returner første ansatt i gruppen med ansatte
                return result.ElementAt(0);
            }
        }

       
        /**
         * Metode som henter alle ansatte
         */
        public List<EmployeeEntity> GetEmployees()
        {
            //spørring
            var query = @"SELECT e.emp_id, e.name, e.role_id, r.role_id, r.role_name, t.team_id, t.team_name, t.team_lead_id FROM Employee AS e 
            INNER JOIN Role AS r ON e.role_id = r.role_id INNER JOIN TeamList AS tl ON tl.emp_id = e.emp_id 
            INNER JOIN Team AS t ON tl.team_id = t.team_id";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                var emp = connection.Query<EmployeeEntity, RoleEntity, TeamEntity, EmployeeEntity>(query, (employee, role, team) =>
                {
                    //variabel role i koden kobles med variabel role i databasen
                    employee.role = role;
                    if (employee.teams == null)
                    {
                        employee.teams = new List<TeamEntity>();
                    }
                    //legg til team
                    employee.teams.Add(team);
                    //returner ansatt
                    return employee;
                }, splitOn: "role_id, team_id");
                //grupper ansatte
                var result = emp.GroupBy(e => e.emp_id).Select(employee =>
                {
                    var groupedEmployee = employee.First();
                    groupedEmployee.teams = employee.Select(e => e.teams.Single()).ToList();
                    return groupedEmployee;
                });

                //returner liste med ansatte
                return result.ToList();
            }
        }
      //Metode som henter en liste over ansatte som SelectListItem så de fungerer med checkbox 
        public List<SelectListItem> GetEmployeeSelectList()
        {
         //Spørring
            var query = @"SELECT emp_id, name FROM Employee";
            List<SelectListItem> list = new List<SelectListItem>();
            //Kobler spørring til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = query;
                var reader = command.ExecuteReader();
                //Dapper mapper ikke automatisk til en SelectListItem så vi må gjøre det på gamlemåten
                while (reader.Read())
                {
                    var item = new SelectListItem();
                    item.Value = reader[0].ToString();
                    item.Text = reader[1].ToString();
                    list.Add(item);
                }
                connection.Close();
                //Returnerer listen
                return list;
            }
        }

    //metode som henter team basert på team_id
        public TeamEntity GetTeam(int team_id)
        {
            //spørring
            var query = @"SELECT t.team_id, t.team_name, t.team_lead_id, e.emp_id, e.name, r.role_id, r.role_name FROM Team AS t INNER JOIN 
            TeamList AS tl ON t.team_id = tl.team_id INNER JOIN Employee AS e ON e.emp_id = tl.emp_id INNER JOIN Role AS r ON
            e.role_id = r.role_id WHERE t.team_id = @team_id";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
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
                    //grupperer team
                    var groupedTeams = team.First();
                    groupedTeams.employees = team.Select(e => e.employees.Single()).ToList();
                    return groupedTeams;
                });

                //returner første team i gruppen med team
                return result.ElementAt(0);
            }
        }

        /**
         * Metode som henter teamene
         */
        public List<TeamEntity> GetTeams()
        {
            //Spørring
            var query = @"SELECT t.team_id, t.team_name, t.team_lead_id, e.emp_id, e.name, r.role_id, r.role_name FROM Team AS t INNER JOIN 
            TeamList AS tl ON t.team_id = tl.team_id INNER JOIN Employee AS e ON e.emp_id = tl.emp_id INNER JOIN Role AS r ON
            e.role_id = r.role_id";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
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
                //grupper team
                var result = team.GroupBy(e => e.team_id).Select(team =>
                {
                    var groupedTeams = team.First();
                    groupedTeams.employees = team.Select(e => e.employees.Single()).ToList();
                    return groupedTeams;
                });
                //returner gruppen med team
                return result.ToList();
            }
        }
    }
}