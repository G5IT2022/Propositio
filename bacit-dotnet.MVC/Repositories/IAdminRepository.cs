using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories
{
    /**
     * Repositository for admin
     * ansvar for lagring av employees
     */
    public interface IAdminRepository
    {
        string AuthorizeUser(int emp_id);
        bool UserExists(int emp_id);
        public byte[] GetSalt(int emp_id);
        EmployeeEntity AuthenticateUser(int emp_id, string password);

        int CreateCategory();
        List<RoleEntity> GetAllRoles();
        string CreateNewRole(RoleEntity role);
        int DeleteRole(int role_id);
        int CreateTeam();
        int CreateEmployee();
        int UpdateEmployee();
    }
}
