using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace DataAccess;

public class DapperDBConnectionDummy
{
    private readonly string _connectionString;

    public DapperDBConnectionDummy(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MariaDbConnection")!;
    }

    public MySqlConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}