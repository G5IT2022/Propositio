using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Team
{
    public interface ITeamRepository
    {
        //Generere nytt id for nytt team
        public int GetNewTeamId();
        //Lage et nytt team
        public int Create(TeamEntity team);
        List<TeamEntity> GetAll();

        //Henter en liste over alle teams for en ansatt
        List<TeamEntity> Get(int id);
        List<EmployeeEntity> GetEmployeesForTeam(int team_id);
        TeamEntity GetTeam(int id);
    }
}
