using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;

namespace DataAccess
{
    public class SaksbehandlerDBConnection
    {
        private readonly string _connectionString;

        public SaksbehandlerDBConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MariaDbConnection")!;
        }

        public IDbConnection CreateConnection()
            => new MySqlConnection(_connectionString);
    }
}