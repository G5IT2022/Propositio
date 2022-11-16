using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;

using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using MySqlConnector;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace bacit_dotnet.MVC.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ISqlConnector sqlConnector;

        public AdminRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }
        //Sjekk bruker med emp_id og passord om brukeren er registrert
        /**
         * Denne metoden er for å sjekke dersom passord til brukeren er registrert
         * @Parameter emp_id, string password
         * @Return employee
         */
        public EmployeeEntity AuthenticateUser(int emp_id, string password)
        {
            var query = @"SELECT emp_id, name FROM Employee WHERE emp_id = @emp_id AND passwordhash = @password";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var emp = connection.QueryFirstOrDefault<EmployeeEntity>(query, new { emp_id = emp_id, password = password });
                return emp;
            }
        }
       
        /**
         * Denne metoden gjør at du kan sjekke rollen til brukeren
         * @Parameter emp_id
         * @Return rollen
         */
        public string AuthorizeUser(int emp_id)
        {
            var authorizeUser = @"SELECT ar.authorization_role_name FROM AuthorizationRole AS ar
            INNER JOIN Employee AS e ON  e.authorization_role_id = ar.authorization_role_id WHERE e.emp_id = @emp_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var role = connection.QueryFirstOrDefault<string>(authorizeUser, new { emp_id = emp_id });
                return role;
            }
        }
        //registrer kategori 
        public int CreateCategory()
        {
            throw new NotImplementedException();
        }


        /**
         * Denne metoden gjør at du kan lage en ny bruker i databasen
         * @Parameter EmployeeEntity som består av alle nødvendige attributter om en ansatt
         * @Return ny bruker/ansatt
        /**
         * Metode for å registrere ansatte 
         */
        public int CreateEmployee(EmployeeEntity emp)
        {
            //spørring
            var query = @"INSERT INTO Employee(emp_id, name, passwordhash,salt, role_id, authorization_role_id) VALUES (@emp_id, @name, @passwordhash, @salt, @role_id, @authorization_role_id)";
            var firstTeamQuery = @"INSERT INTO TeamList(emp_id, team_id) VALUES (@emp_id, 1)";
            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                int result = connection.Execute(query, new { emp.emp_id, emp.name, emp.passwordhash, emp.salt, emp.role_id, emp.authorization_role_id });
                //Legger til i "uten team" teamet
                if (result == 1)
                {
                    connection.Execute(firstTeamQuery, new { emp_id = emp.emp_id });
                }
                return result;
            }
        }
        
       /**
         * Denne metoden er for å hente rollelisten.         
         * @Return rollelisten
         */
         public List<RoleEntity> GetAllRoles()
        {
            var query = @"SELECT role_id, role_name FROM Role";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var roles = connection.Query<RoleEntity>(query);
                return roles.ToList();
            }
        }
        
        /**
         * Denne metoden er for å legge til nye roller i databasen
         * @Paramter RoleEntity
         * @Return ny rolle
         */
        public string CreateNewRole(RoleEntity role)
        {

            var query = @"INSERT INTO Role(role_name) VALUES (@role_name)";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                string result = connection.QueryFirstOrDefault(query, new { role.role_name });
                return result;
            }
        }

        /**
         * Denne metoden er for å slette rolle
         * @Parameter role_id
         * @Return rollen blir slettet
         */
        public int DeleteRole(int role_id)
        {
            var query = @"DELETE FROM Role WHERE role_id = @role_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var affectedRows = connection.Execute(query, new { role_id = role_id });
                return affectedRows;
            }

        }
        
        //registrer team
        public int CreateTeam()
        {
            throw new NotImplementedException();
        }

        public List<SelectListItem> GetRoleSelectList()
        {
            //Spørring
            var query = @"SELECT role_id, role_name FROM Role";
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

        /**
* Metode som henter salt(tilfeldig data) basert på ansatte
*/
        public byte[] GetSalt(int emp_id)
        {
            //spørring
            var query = @"SELECT salt FROM Employee WHERE emp_id = @emp_id";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection){
                var result = connection.QueryFirstOrDefault<byte[]>(query, new { emp_id = emp_id });

                //returnerer streng med tilfeldig data knyttet til brukeren
                return result;
            }
        }

        public int UpdateEmployee()
        {
            throw new NotImplementedException();
        }

       
        /**
         * Denne metoden er for å sjekke om bruker finnes i systemet
         * @Parameter emp_id
         * @Return true/false
         */
        public bool UserExists(int emp_id)
        {
            //Sjekk om bruker finnes med emp_id
            var query = @"SELECT emp_id FROM Employee WHERE emp_id = @emp_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var user = connection.QueryFirstOrDefault<EmployeeEntity>(query, new { emp_id = emp_id });
                if (user != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
