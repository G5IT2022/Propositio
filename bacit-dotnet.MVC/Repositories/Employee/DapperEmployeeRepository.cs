﻿using bacit_dotnet.MVC.DataAccess;
using bacit_dotnet.MVC.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using MySqlConnector;

namespace bacit_dotnet.MVC.Repositories.Employee
{
    public class DapperEmployeeRepository : IEmployeeRepository
    {

        private readonly ISqlConnector sqlConnector;

        public DapperEmployeeRepository(ISqlConnector sqlConnector)
        {
            this.sqlConnector = sqlConnector;
        }
        public void Delete(EmployeeEntity emp)
        {
            throw new NotImplementedException();
        }


        public List<EmployeeEntity> GetAll()
        {
            using (var connection = sqlConnector.GetDbConnection() as MySqlConnection)
            {
                var users = connection.Query<EmployeeEntity>("SELECT * FROM Employee;"); //Regular Dapper
                return users.ToList();
            }
        }

        public void Save(EmployeeEntity emp)
        {
            throw new NotImplementedException();
        }
    }
}