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
        int CreateRole();
        int CreateTeam();
        int CreateEmployee();
        int UpdateEmployee();
    }
}
