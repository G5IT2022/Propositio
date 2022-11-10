﻿using bacit_dotnet.MVC.Entities;
using MySqlConnector;
using System.Data;
using System.Data.Common;

namespace bacit_dotnet.MVC.DataAccess
{
    public class SqlConnector : ISqlConnector
    {
        private readonly IConfiguration config;

        public SqlConnector(IConfiguration config)
        {
            this.config = config;
        }
        //kobler til databasen
        public IDbConnection GetDbConnection()
        {
            var cnn = config.GetConnectionString("propositio");
            var connect = new MySqlConnection(config.GetConnectionString("propositio"));
            return new MySqlConnection(config.GetConnectionString("propositio"));
        }
          
    }
}
