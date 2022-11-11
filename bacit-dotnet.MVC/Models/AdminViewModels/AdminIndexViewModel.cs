using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Models.AdminViewModels
{
    public class AdminIndexViewModel
    {
        public List<EmployeeEntity> employees;
        public List<TeamEntity> teams;
        public List<CategoryEntity> categories;
        public List<RoleEntity> roles;
    }
}
