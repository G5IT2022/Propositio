using bacit_dotnet.MVC.Entities;
using System;

namespace bacit_dotnet.MVC.Models.Statistics
{
    public partial class StatisticsViewModel
    {
        public int totalNumberOfcategoriesForSuggestions { get; set; }
        public List<int> ListTopNumberOfSuggestionsOfTopFiveEmployees { get; set; }
        public List<int> ListNumberOfSuggestionsPerStatus { get; set; }
        public List<int> ListNumberOfSuggestionPerTeam { get; set; }
        public List<int> ListNumberOfSuggestionsPerCategory { get; set; }
        public List<EmployeeEntity> employees { get; set; }
        public List<SuggestionEntity> statuses { get; set; }
        public List<TeamEntity> teams { get; set; }
        public List<CategoryEntity> categories { get; set; }
    }
}
