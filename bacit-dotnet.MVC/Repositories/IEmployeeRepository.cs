using bacit_dotnet.MVC.Entities;
using bacit_dotnet.MVC.Models.AdminViewModels.TeamModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bacit_dotnet.MVC.Repositories
{
    /**
     * Dette repositoryet har ansvar for ALT som har med ansatte å gjøre. 
     * I det vi hadde før tilsvarer dette repositoryet
     * - Employeerepository
     * - Teamrepository
     * - Rolerepository
     * 
     * Forskjellen fra dette og det gamle er at det "fyller" alle objektene inn i de andre objektene.
     * **/
    public interface IEmployeeRepository
    {
        //Henter en enkelt ansatt basert på employeeid, returnerer en EmployeeEntity med en liste over team de er med i og rolle. 
        public EmployeeEntity GetEmployee(int emp_id);
        public List<SelectListItem> GetEmployeeSelectList();
        public List<EmployeeEntity> GetEmployees();
        public TeamEntity GetTeam(int team_id);
        public List<TeamEntity> GetTeams();
        public int CreateEmployee(EmployeeEntity employee);

        //Alle er for Team som du kan bruke for å lage nytt team, hente informasjon for et team, sjekke og slette team.
        public TeamEntity CreateNewTeam(AdminNewTeamModel model);       
        public TeamEntity GetTeamByName(string name);
        public bool InsertMemberToTeam(int team_id, int emp_id);
        public bool CheckExistedMember(int team_id, int employeeID);
        public int DeleteTeam(int team_id);
    }
    
}
