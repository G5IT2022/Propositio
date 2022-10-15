using bacit_dotnet.MVC.Entities;

namespace bacit_dotnet.MVC.Repositories.Role
{
    public interface IRoleRepository
    {
        void Create(RoleEntity role);
        List<RoleEntity> GetAll();
        RoleEntity Get(int role_id);
        int Update(RoleEntity role);
        int Delete(RoleEntity role);
    }
}