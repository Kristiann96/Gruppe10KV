using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Dapper;
using System.Data;

namespace DataAccess
{
    public class IncidentFormDBContext
    {
        private readonly string _connectionString;

        public IncidentFormDBContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MariaDbConnection")!;
        }

        public IDbConnection CreateConnection()
            => new MySqlConnection(_connectionString);
    }
}


