using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Team
{
    public interface ITeamRepository
    {
        List<TeamEntity> GetALl();

        //Henter en liste over alle teams for en ansatt
        List<TeamEntity> Get(int id);

    }
}
