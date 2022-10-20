using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Team
{
    public interface ITeamRepository
    {
        List<TeamEntity> GetAll();

        //Henter en liste over alle teams for en ansatt
        List<TeamEntity> Get(int id);
        List<EmployeeEntity> GetEmployeesForTeam(int team_id);
        TeamEntity GetTeam(int id);
    }
}
