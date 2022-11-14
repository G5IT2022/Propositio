using bacit_dotnet.MVC.Entities;
using System;

namespace bacit_dotnet.MVC.Models.Statistics
{
    public partial class StatisticsViewModel
    {
        public List<int> ListTopNumberOfSuggestionsOfTopThreeEmployees { get; set; }
        public List<EmployeeEntity> employees { get; set; }
        public List<int> ListNumberOfSuggestionsPerStatus { get; set; }
        public List<SuggestionEntity> statuses { get; set; }
        public List<int> ListNumberOfSuggestionPerTeam { get; set; }
        public List<TeamEntity> teams { get; set; }
    }
}
