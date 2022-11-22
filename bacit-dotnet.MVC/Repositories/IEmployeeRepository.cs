using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models.AdminViewModels;
using bacit_dotnet.MVC.Models.AdminViewModels.TeamModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bacit_dotnet.MVC.Repositories
{
    /**
     * Employeerepository
     * Dette repositoryet har ansvaret for alle read funskjoner om employee og team.
     * **/
    public interface IEmployeeRepository
    {
        //Henter en enkelt ansatt basert på employeeid, returnerer en EmployeeEntity med en liste over team de er med i og rolle. 
        public EmployeeEntity GetEmployee(int emp_id);
        public List<SelectListItem> GetEmployeeSelectList();
        public List<EmployeeEntity> GetEmployees();
        public TeamEntity GetTeam(int team_id);
        public List<TeamEntity> GetTeams();
    }
}
