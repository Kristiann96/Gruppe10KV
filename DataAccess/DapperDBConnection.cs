﻿using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Dapper;
using System.Data;

namespace DataAccess
{
    public class DapperDBConnection
    {
        private readonly string _connectionString;

        public DapperDBConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MariaDbConnection")!;
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}


