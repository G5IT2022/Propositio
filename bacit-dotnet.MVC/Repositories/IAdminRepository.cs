using bacit_dotnet.MVC.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bacit_dotnet.MVC.Repositories
{
    /**
     * Repositository for admin
     * ansvar for lagring av employees
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
        public string CreateNewRole(RoleEntity role);
        public int DeleteRole(int role_id);
        public List<RoleEntity> GetAllRoles();
        public List<SelectListItem> GetRoleSelectList();

    }
}
