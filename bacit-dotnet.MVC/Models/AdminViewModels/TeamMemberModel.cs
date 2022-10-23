using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Models.AdminViewModels
{
    public class TeamMemberModel
    {
        public List<EmployeeEntity> employees { get; set; }
        public List<TeamEntity> teams { get; set; }
        public List<RoleEntity> roles { get; set; }
    }
}
