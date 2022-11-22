using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models.AdminViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bacit_dotnet.MVC.Repositories
{
    /**
     * Repositository for admin
     * Ansvar for Create, Update og Delete av alle entiteter
     */
    public interface IAdminRepository
    {
        public string AuthorizeUser(int emp_id);
        public bool UserExists(int emp_id);
        public byte[] GetSalt(int emp_id);
        public EmployeeEntity AuthenticateUser(int emp_id, string password);
        public int CreateEmployee(EmployeeEntity emp);
        public int UpdateEmployee(EmployeeEntity emp);
        public int CreateTeam();
        public int UpdateTeam(TeamEntity team);
        public int DeleteTeamMember(int emp_id, int team_id);
        public string CreateNewCategory(string category);
        public int DeleteCategory(int category_id);
        public RoleEntity CreateNewRole(AdminIndexViewModel model);
        public int DeleteRole(int role_id);
        public RoleEntity GetRoleByName(string name);
        public List<RoleEntity> GetAllRoles();
        public List<SelectListItem> GetRoleSelectList();
        /*
* Categories
*/

        //Read

        /// <summary>
        /// Returns a list of all categories in the db, mapped to Categoryentities
        /// </summary>
        /// <returns>A list of CategoryEntity<returns>
        public List<CategoryEntity> GetAllCategories();

    }
}
