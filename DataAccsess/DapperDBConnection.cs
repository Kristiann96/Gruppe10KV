using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace DataAccess;

public class DapperDBConnection
{
    private readonly string _connectionString;

    public DapperDBConnection(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MariaDbConnection_kartverket")!;
    }

    public MySqlConnection CreateConnection()
    {
        var connection = new MySqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}