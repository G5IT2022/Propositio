using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories
{
    public interface IStatisticsRepository
    {
        public List<int> ListNumberOfSuggestionsPerTeam();
        public List<TeamEntity> ListTeams();
    }
}
