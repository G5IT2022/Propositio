using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;

namespace bacit_dotnet.MVC.Repositories.Role
{
    public class DapperRoleRepository : IRoleRepository
    {
        private readonly ISqlConnector sqlConnector;

        public DapperRoleRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }

        public void Create(RoleEntity role)
        {
            throw new NotImplementedException();
        }

      
        public RoleEntity Get(int role_id)
        {
            throw new NotImplementedException();
        }

        public List<RoleEntity> GetAll()
        {
            var query = @"SELECT * FROM Role";
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var roles = connection.Query<RoleEntity>(query);
                return roles.ToList();
            }

        }

        public int Update(RoleEntity role)
        {
            throw new NotImplementedException();
        }

        public int Delete(RoleEntity role)
        {
            throw new NotImplementedException();
        }

    }
}
