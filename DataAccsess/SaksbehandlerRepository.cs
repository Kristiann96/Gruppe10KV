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


    public async Task<(SaksbehandlerModel? Saksbehandler, PersonModel? Person)> HentSaksbehandlerNavnAsync(int innmeldingId)
    {
        using var connection = _dbConnection.CreateConnection();
        var sql = @"
        SELECT 
            s.saksbehandler_id AS SaksbehandlerId,
            s.stilling AS Stilling,
            p.person_id AS PersonId,
            p.fornavn AS Fornavn,
            p.etternavn AS Etternavn
        FROM innmelding im
        LEFT JOIN saksbehandler s ON im.saksbehandler_id = s.saksbehandler_id
        LEFT JOIN person p ON s.person_id = p.person_id
        WHERE im.innmelding_id = @innmeldingId";

        var result = await connection.QueryAsync<SaksbehandlerModel, PersonModel,
            (SaksbehandlerModel?, PersonModel?)>(
            sql,
            (saksbehandler, person) => (saksbehandler, person),
            new { innmeldingId },
            splitOn: "PersonId"
        );

        var (saksbehandler, person) = result.FirstOrDefault((null, null));
        return (saksbehandler, person);
    }
}