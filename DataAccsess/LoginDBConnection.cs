using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    internal class LoginDbConnection
    {
        private readonly string _connectionString;

        public LoginDbConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MariaDbConnection_login_server")!;
        }

        public MySqlConnection CreateConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
