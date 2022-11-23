using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models.AdminViewModels;
using bacit_dotnet.MVC.Models.AdminViewModels.TeamModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bacit_dotnet.MVC.Repositories
{
    /**
     * Repositository for admin
     * Ansvar for Create, Update og Delete av alle entiteter
     */
    public interface IAdminRepository
    {

        //Category (Create, Read, Delete)
        //Create
        public int CreateCategory(CategoryEntity category);
        
        //Read
        public bool CategoryExists(CategoryEntity category);
        public List<CategoryEntity> GetAllCategories();
        public int GetSuggestionsWithCategoryCount(int category_id);
      
        //Delete
        public int DeleteCategory(int category_id);


        //Role(Create, Read, Delete)

        //Create
        public int CreateRole(RoleEntity role);

        //Read
        public bool RoleExists(RoleEntity role);
        public List<RoleEntity> GetAllRoles();
        public List<SelectListItem> GetRoleSelectList();

        //Delete
        public int DeleteRole(int role_id);


        //Employee (Create, Update, Delete)

        //Create
        public int CreateEmployee(EmployeeEntity emp);

        //Update
        public int UpdateEmployee(EmployeeEntity emp);

        //Delete

        public int DeleteEmployee(int emp_id);


        //Employee (Authetnication and authorization)
        public string AuthorizeUser(int emp_id);
        public bool UserExists(int emp_id);
        public byte[] GetSalt(int emp_id);
        public EmployeeEntity AuthenticateUser(int emp_id, string password);

        //Team (Create, Update, Delete)

        //Create

        //Alle er for Team som du kan bruke for å lage nytt team, hente informasjon for et team, sjekke og slette team.
        public TeamEntity CreateNewTeam(AdminNewTeamModel model);
        public TeamEntity GetTeamByName(string name);
        public int GetTeamMembersInTeamCount(int team_id);
        public bool InsertMemberToTeam(int team_id, int emp_id);
        public bool CheckExistedMember(int team_id, int employeeID);
        public int DeleteTeam(int team_id);
        public int CreateTeam();

        //Update
        public int UpdateTeam(TeamEntity team);
        public int DeleteTeamMember(int emp_id, int team_id);

        /*
* Categories
*/

        //Read

        /// <summary>
        /// Returns a list of all categories in the db, mapped to Categoryentities
        /// </summary>
        /// <returns>A list of CategoryEntity<returns>

    }
}
