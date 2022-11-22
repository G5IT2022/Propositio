using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models.AdminViewModels;
using bacit_dotnet.MVC.Models.AdminViewModels.TeamModels;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using MySqlConnector;
using MySqlX.XDevAPI.Common;
using System.Data;
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

        /**
        * Denne metoden lager en ny kategori i databasen
        * @Parameter En Category Entity
        * @Return 1 hvis fullført, 0 hvis ikke
        */
        public int CreateCategory(CategoryEntity category)
        {
            var query = @"INSERT INTO Category(category_name) VALUES (@category_name)";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var result = connection.Execute(query, new { category_name = category.category_name });
                return result;
            }
        }


        /**
         * Denne metoden sjekker om en rolle allerede eksisterer i databasen
         * Brukt for verifisering
         * @Parameter en Role Entity du vil sjekke
         * @Return true hvis den finnes, false hvis ikke
         */

        public bool CategoryExists(CategoryEntity category)
        {
            var query = @"SELECT category_name FROM Category WHERE lower(category_name) = @category_name";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var newCategory = connection.QueryFirstOrDefault<CategoryEntity>(query, new { category_name = category.category_name.ToLower() });
                if (newCategory == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }

        /**
        * Denne metoden henter alle kategoriene
        * @Return List av kategorier
        */
        public List<CategoryEntity> GetAllCategories()
        {
            //spørring
            var query = @"SELECT Category.category_id, Category.category_name FROM Category";
            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                //kobler spørring til databasen
                var categories = connection.Query<CategoryEntity>(query);
                //returnerer alle kategorier --> kategoriliste
                return categories.ToList();
            }
        }

        /**
         * Returnerer antall forslag med denne kategorien, trengs for å ikke slette kategorier som tilhører et forslag
         * @Return antall forslag i form av en int
         */

        public int GetSuggestionsWithCategoryCount(int category_id)
        {
            var query = "SELECT COUNT(*) FROM SuggestionCategory WHERE category_id = @category_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var result = connection.QueryFirstOrDefault<int>(query, new { category_id = category_id });
                return result;
            }
        }

        /**
        * Denne metoden er for å slette en kategori i databasen
        * @Parameter category_id
        * @Return 1 hvis fullført, 0 hvis ikke
        */
        public int DeleteCategory(int category_id)
        {
            var query = @"DELETE FROM Category WHERE category_id = @category_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var result = connection.Execute(query, new { category_id = category_id });
                return result;
            }
        }

        /**
         * Denne metoden lager en ny rolle i databasen
         * @Paramter RoleEntity
         * @Return 1 hvis fullført, 0 hvis ikke
         */

        public int CreateRole(RoleEntity role)
        {
            var query = @"INSERT INTO Role(role_name) VALUES (@role_name)";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var result = connection.Execute(query, new { role_name = role.role_name });
                return result;
            }
        }

        /**
         * Denne metoden sjekker om en rolle allerede eksisterer i databasen
         * Brukt for verifisering
         * @Parameter en Role Entity du vil sjekke
         * @Return true hvis den finnes, false hvis ikke
         */

        public bool RoleExists(RoleEntity role)
        {
            var query = @"SELECT role_name FROM Role WHERE lower(role_name) = @role_name";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var newRole = connection.QueryFirstOrDefault<RoleEntity>(query, new { role_name = role.role_name.ToLower() });
                if (newRole == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
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
        * Denne metoden er for å hente utvalgte rolle listen
        * @Return rollelisten som en liste med SelectListItem
        */
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

        /**
         * Denne metoden gjør at du kan lage en ny bruker i databasen
         * @Parameter EmployeeEntity som består av alle nødvendige attributter om en ansatt
         * @Return 1 hvis fullført, 0 hvis ikke
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
         * Denne metoden oppdaterer ansatte i databasen
         * @Parameter EmployeeEntity som du vil oppdatere
         * @Return antall rader som ble endret (Vi har ingen god måte å verifisere at alle ble oppdatert enda)
         */
        public int UpdateEmployee(EmployeeEntity emp)
        {
            var query = @"UPDATE Employee SET name = @name, passwordhash = @passwordhash, role_id = @role_id WHERE emp_id = @emp_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var affectedRows = connection.Execute(query, new { name = emp.name, passwordhash = emp.passwordhash, role_id = emp.role_id, emp_id = emp.emp_id });
                return affectedRows;
            }
        }

        /**
         * Denne metoden sletter en ansatt fra databasen
         * @Paramter emp_id til den du vil slette
         * @Return 1 hvis fullført, 0 hvis ikke
         */

        public int DeleteEmployee(int emp_id)
        {
            return 1;
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

        /**
        * Metode som henter salt(tilfeldig data) basert på ansatte
        */
        public byte[] GetSalt(int emp_id)
        {
            //spørring
            var query = @"SELECT salt FROM Employee WHERE emp_id = @emp_id";

            //kobler til databasen
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var result = connection.QueryFirstOrDefault<byte[]>(query, new { emp_id = emp_id });

                //returnerer streng med tilfeldig data knyttet til brukeren
                return result;
            }
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
    * Denne metoden er for å lage et nytt team i databasen
* Denne metoden fungerer sammen med metoden GetTeamByName
* Når du lager et nytt team, blir nytt team navn og en teamleder lagret i databasen
* Et nytt teamnavn skal bli lagret i databasen dersom det ikke har eksistert før
* @Parameter AdminNewTeamModel som består av attributter team_name og team_lead_id
* @Return nytt teamnavn
*/
        public TeamEntity CreateNewTeam(AdminNewTeamModel model)
        {
            var query = $"INSERT INTO Team(team_name,team_lead_id) VALUES(@team_name,@team_lead_id)";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var result = connection.QueryFirstOrDefault(query, new { model.team_name, model.team_lead_id });
            }
            return GetTeamByName(model.team_name);
        }



        //registrer team
        public int CreateTeam()
        {
            throw new NotImplementedException();
        }

        /**
         * Denne metoden gjør at:
         * 1. Du kan legge til flere ansatte i teamet
         * 2. Du kan endre Teamnavn og teamleder
         * @Parameter TeamEntity
         * @Return team med oppdatert informasjon
         */
        public int UpdateTeam(TeamEntity team)
        {
            //var query = @"UPDATE TeamList SET emp_id = @emp_id WHERE team_id = @team_id";
            var query = @"INSERT INTO TeamList(emp_id, team_id) VALUES (@emp_id, @team_id)";
            var updateTeamleader = @"UPDATE Team SET team_lead_id = @team_lead_id, team_name = @team_name WHERE team_id = @team_id";
            using (var connection = sqlConnector.GetDbConnection())
            {
                var result = 0;
                foreach (EmployeeEntity emp in team.employees)
                {
                    result = connection.Execute(query, new { emp_id = emp.emp_id, team_id = team.team_id });
                }
                result += connection.Execute(updateTeamleader, new { team_lead_id = team.team_lead_id, team_name = team.team_name, team_id = team.team_id });
                return result;
            }
        }
        /**
         * Denne metoden er for å slette et medlem i teamet
         * @Parameter emp_id
         * @Return emp_id i teamet blir slettet
         */
        public int DeleteTeamMember(int emp_id, int team_id)
        {
            var query = @"DELETE FROM TeamList WHERE emp_id = @emp_id AND team_id = @team_id";

            //var query3 = @"Update TeamList Set status=0 WHERE emp_id = @emp_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var affectedRows = connection.Execute(query, new { emp_id = emp_id, team_id = team_id });
                return affectedRows;
            }
        }



        /**
         * Denne metoden gjør at du kan hente nytt teamnavn for å sjekke dersom det teamnavnet har eksistert i databasen.
         * @Parameter en string name av team
         * @Return teamnavn.
         */
        public TeamEntity GetTeamByName(string name)
        {
            var query = @"SELECT * FROM Team as t WHERE t.team_name = @team_name";

            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var result = connection.QueryFirstOrDefault(query, new { team_name = name });
                if (result != null)
                {
                    TeamEntity team = new TeamEntity();
                    team.team_id = result.team_id;
                    team.team_name = result.team_name;
                    team.team_lead_id = result.team_lead_id;
                    return team;
                }
                return result;
            }
        }
        /**
         * Denne metoden med boolean datatype som fungerer sammen med metoden CheckExistedMember
         * Metoden gjør at når du velger noen ansatte fra checkbox, går det til CheckExistedMember
         * for å sjekke dersom det nye teamet med list av utvalgte ansatte og utvalgte teamleder har eksistert i TeamList.
         * Hvis ikke, @Returnerer "True", og så blir de lagt til TeamList i databasen.     
         * @Parameter teamnr og ansattnr
         */
        public bool InsertMemberToTeam(int team_id, int emp_id)
        {
            //check exist employee in Team Table
            if (CheckExistedMember(team_id, emp_id))
            {
                var query = $"INSERT INTO TeamList(emp_id,team_id) VALUES(@emp_id, @team_id)";
                using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
                {
                    var result = connection.QueryFirstOrDefault(query, new { team_id, emp_id });
                    //return result;
                }
            }
            return true;
        }
        public bool CheckExistedMember(int team_id, int emp_id)
        {
            var query = @"SELECT * FROM TeamList WHERE team_id=@team_id and emp_id=@emp_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var user = connection.QueryFirstOrDefault<EmployeeEntity>(query, new { team_id = team_id, emp_id = emp_id });
                if (user == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /**
         * Denne metoden er for å slette Team fra databasen.
         * @Parameter team_id
         * @Return 
         */
        public int DeleteTeam(int team_id)
        {
            var query = @"DELETE FROM Team WHERE team_id = @team_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var result = connection.Execute(query, new { team_id = team_id });
                return result;
            }
        }

        public int GetTeamMembersInTeamCount(int team_id)
        {
            var query = @"SELECT COUNT(*) FROM TeamList WHERE team_id = @team_id";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var result = connection.QueryFirstOrDefault<int>(query, new { team_id = team_id });
                return result;
            }
        }





    }
}
