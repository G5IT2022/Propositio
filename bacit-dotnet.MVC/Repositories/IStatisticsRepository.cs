using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories
{
    public interface IStatisticsRepository
    {
        public int TotalNumberCategoriesFromSuggestions();
        public List<int> ListNumberOfSuggestionsPerTeam();
        public List<int> ListNumberOfSuggestionsPerStatus();
        public List<int> ListTopNumberOfSuggestionsOfTopFiveEmployees();
        public List<int> ListNumberOfSuggestionsPerCategory();
        public List<CategoryEntity> ListCategories();
        public List<EmployeeEntity> ListEmployees();
        public List<SuggestionEntity> ListStatuses();
        public List<TeamEntity> ListTeams();
    }
}
