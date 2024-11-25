using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace DataAccess;

public class DapperDBConnection
{
    private readonly string _connectionString;

    public DapperDBConnection(IConfiguration configuration)
    {
        var template = configuration.GetConnectionString("MariaDbConnection_kartverket")!;
        _connectionString = template
            .Replace("${DB_SERVER}", configuration["DbSettings:MariaDb:Server"])
            .Replace("${DB_PORT}", configuration["DbSettings:MariaDb:Port"])
            .Replace("${DB_USER}", configuration["DbSettings:MariaDb:User"])
            .Replace("${DB_PASSWORD}", configuration["DbSettings:MariaDb:Password"]);
    }

    public MySqlConnection CreateConnection()
    {
        var connection = new MySqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}