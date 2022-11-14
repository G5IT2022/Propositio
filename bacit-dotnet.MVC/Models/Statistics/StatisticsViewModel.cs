using bacit_dotnet.MVC.Entities;
using System;

namespace bacit_dotnet.MVC.Models.Statistics
{
    public partial class StatisticsViewModel
    {
        public List<int> ListNumberOfSuggestionPerTeam { get; set; }
        public List<TeamEntity> teams { get; set; }
    }
}
