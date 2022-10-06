using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;

namespace bacit_dotnet.MVC.Repositories
{
    public class DapperRoleRepository : IRoleRepository
    {
        private readonly ISqlConnector sqlConnector;

        public DapperRoleRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }

        public List<RoleEntity> GetAllRoles()
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var roles = connection.Query<RoleEntity>("SELECT * FROM Role;");
                return roles.ToList();
            }

        }
    }
}
