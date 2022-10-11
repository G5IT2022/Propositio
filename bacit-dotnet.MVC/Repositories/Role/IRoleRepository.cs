using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Role
{
    public interface IRoleRepository
    {
        List<RoleEntity> GetAllRoles();
    }
}