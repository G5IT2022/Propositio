using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Models.AdminViewModels
{
    public class AdminIndexViewModel
    {
        public SuggestionEntity suggestion { get; set; }
        public List<EmployeeEntity> employees;
        public List<TeamEntity> teams;
        public List<CategoryEntity> categories;
        public List<RoleEntity> roles;
        public string role_name { get; set; }
        public int role_id { get; set; }
        public int category_id { get; set; }
        public string category_name { get; set; }
        public EmployeeEntity teamleader { get; set; }

    }
}
