using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories
{
    public interface IAdminRepository
    {
        string AuthorizeUser(int emp_id);
        bool UserExists(int emp_id);
        EmployeeEntity AuthenticateUser(int emp_id, string password);
    }
}
