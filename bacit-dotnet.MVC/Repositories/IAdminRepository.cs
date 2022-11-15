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

        public int CreateCategory();
        public int CreateRole();
        public int CreateTeam();
        public int CreateEmployee(EmployeeEntity emp);
        public int UpdateEmployee();
        public List<SelectListItem> GetRoleSelectList();
    }
}
