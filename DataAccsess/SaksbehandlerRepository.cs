using Dapper;
using MySqlConnector;
using System.Threading.Tasks;
using Models.Entities;
using Interface;

namespace DataAccess;

public class SaksbehandlerRepository : ISaksbehandlerRepository
{
    private readonly DapperDBConnection _dbConnection;

    public SaksbehandlerRepository(DapperDBConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }


    public async Task<List<(SaksbehandlerModel, PersonModel)>> HentAlleSaksbehandlereMedPersonAsync()
    {
        using var connection = _dbConnection.CreateConnection();
        var sql = @"
        SELECT 
            s.saksbehandler_id AS SaksbehandlerId,
            s.person_id AS PersonId,
            p.fornavn AS Fornavn,
            p.etternavn AS Etternavn
        FROM saksbehandler s
        JOIN person p ON s.person_id = p.person_id;";

        var result = await connection.QueryAsync<SaksbehandlerModel, PersonModel, (SaksbehandlerModel, PersonModel)>(
            sql,
            (saksbehandler, person) =>
            {
                return (saksbehandler, person); 
            },
            splitOn: "PersonId" 
        );

        return result.ToList();
    }
}