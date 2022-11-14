using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories
{
    public interface IStatisticsRepository
    {
        public List<int> ListNumberOfSuggestionsPerTeam();
        public List<int> ListNumberOfSuggestionsPerStatus();
        public List<int> ListTopNumberOfSuggestionsOfTopThreeEmployees();
        public List<EmployeeEntity> ListEmployees();
        public List<SuggestionEntity> ListStatuses();
        public List<TeamEntity> ListTeams();
    }
}
