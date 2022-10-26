using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Models.Test
{
    public class TestViewModel
    {


        public EmployeeEntity starEmployee { get; set; }
        public List<EmployeeEntity> employees { get; set; }

        public TeamEntity starTeam { get; set; }
        public List<TeamEntity> teams { get; set; }

        public SuggestionEntity starSuggestion { get; set; }
        public List<SuggestionEntity> suggestions
        {
            get; set;
        }
    }
}
