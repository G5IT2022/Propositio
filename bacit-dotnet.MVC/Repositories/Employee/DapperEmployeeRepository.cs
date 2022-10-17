using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using MySqlConnector;

namespace bacit_dotnet.MVC.Repositories.Employee
{
    public class DapperEmployeeRepository : IEmployeeRepository
    {

        private readonly ISqlConnector sqlConnector;

        public DapperEmployeeRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }
        public int Create(EmployeeEntity emp)
        {
            var query = @"INSERT INTO Employee(emp_id, name, passwordhash,salt, role_id, authorization_role_id) VALUES (@emp_id, @name, @passwordhash, @salt, @role_id, @authorization_role_id)";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                int result = connection.Execute(query, new {emp_id = emp.emp_id, name = emp.name, passwordhash = emp.passwordhash, salt = emp.salt, role_id = emp.role_id, authorization_role_id = emp.authorization_role_id});
                return result;
            }
        }
        public EmployeeEntity DummyAuthenticate(int emp_id, string password)
        {
            
            var query = @"SELECT * FROM Employee WHERE emp_id = @emp_id AND passwordhash = @passwordhash";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                EmployeeEntity emp = connection.QueryFirstOrDefault<EmployeeEntity>(query, new { emp_id = emp_id, passwordhash = password });
                return emp;
            }
        }

        public string GetEmployeeRoleName(int authorization_role_id)
        {
            var query = @"SELECT authorization_role_name FROM AuthorizationRole WHERE authorization_role_id = @authorization_role_id";
                using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var role = connection.QueryFirstOrDefault<string>(query, new { authorization_role_id = authorization_role_id });
                return role;
            }
        }
        public EmployeeEntity Get(int emp_id)
        {
            var query = @"SELECT * FROM Employee WHERE emp_id = @emp_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var emp = connection.QueryFirstOrDefault<EmployeeEntity>(query, new { emp_id = emp_id });
                return emp;
            }
        }

        public List<EmployeeEntity> GetAll()
        {
            var query = @"SELECT * FROM Employee";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var users = connection.Query<EmployeeEntity>(query); //Regular Dapper
                return users.ToList();
            }
        }
        public int Update(EmployeeEntity emp)
        {
            throw new NotImplementedException();
        }

        public int Delete(EmployeeEntity emp)
        {
            throw new NotImplementedException();
        }

        public EmployeeEntity RealAuthenticate(int emp_id, string password)
        {
            throw new NotImplementedException();
        }

        public bool EmployeeExists(int emp_id)
        {
            return true;
        }
    }
}
