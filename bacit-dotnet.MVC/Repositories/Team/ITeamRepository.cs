using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Team
{
    public interface ITeamRepository
    {
        List<TeamEntity> GetAll();

        //Henter en liste over alle teams for en ansatt
        List<TeamEntity> Get(int id);
        //henter en listen over alle employees
        List<EmployeeEntity> Get(string name);

        List<EmployeeEntity> GetAll();

       
    }

   
}
