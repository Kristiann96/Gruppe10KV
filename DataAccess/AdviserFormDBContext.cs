using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;

namespace DataAccess
{
    public class AdviserFormDBContext
    {
        private readonly string _connectionString;

        public AdviserFormDBContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MariaDbConnection")!;
        }

        public IDbConnection CreateConnection()
            => new MySqlConnection(_connectionString);
    }
}