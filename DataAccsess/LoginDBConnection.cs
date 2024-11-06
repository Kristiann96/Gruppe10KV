using Microsoft.Extensions.Configuration;
using MySqlConnector;

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
