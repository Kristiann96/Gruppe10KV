using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Dapper;
using System.Data;

namespace DataAccess
{
    public class InnmelderDBConnection
    {
        private readonly string _connectionString;

        public InnmelderDBConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MariaDbConnection")!;
        }

        public IDbConnection CreateConnection()
            => new MySqlConnection(_connectionString);
    }
}


