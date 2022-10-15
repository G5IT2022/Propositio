using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Employee
{
    public interface IEmployeeRepository
    {
        void Create(EmployeeEntity emp);

        List<EmployeeEntity> GetAll();
        EmployeeEntity Get(int emp_id);
        int Update(EmployeeEntity emp);
        int Delete(EmployeeEntity emp);
        string GetEmployeeRoleName(int emp_id);

        /**
         * TESTKODE
         * 
         * 
         * **/
        EmployeeEntity DummyAuthenticate(int emp_id, string password);
        EmployeeEntity RealAuthenticate(int emp_id, string password);
    }
}
