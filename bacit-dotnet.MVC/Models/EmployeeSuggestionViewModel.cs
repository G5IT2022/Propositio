using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Models
{
    public class EmployeeSuggestionViewModel
    {
        public List<TeamEntity> teams;
        public List<EmployeeEntity> employees;
        public EmployeeEntity Employee { get; set; }
        public List<SuggestionEntity> suggestions;

    }
}
