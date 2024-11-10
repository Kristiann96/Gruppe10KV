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

    public async Task<bool> ErGyldigSaksbehandlerEpost(string epost)
    {
        const string sql = @"
            SELECT COUNT(1) 
            FROM saksbehandler s
            WHERE s.jobbepost = @Epost";

        using var connection = _dbConnection.CreateConnection();
        var count = await connection.QuerySingleOrDefaultAsync<int>(sql, new { Epost = epost });
        return count > 0;
    }

    public async Task<SaksbehandlerModel?> HentSaksbehandlerMedEpost(string epost)
    {
        const string sql = @"
            SELECT s.saksbehandler_id as SaksbehandlerId,
                   s.person_id as PersonId,
                   s.jobbepost as Jobbepost,
                   s.jobbtelefon as Jobbtelefon,
                   s.stilling as Stilling
            FROM saksbehandler s
            WHERE s.jobbepost = @Epost";

        using var connection = _dbConnection.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SaksbehandlerModel>(sql, new { Epost = epost });
    }

    public async Task<SaksbehandlerModel?> HentSaksbehandler(int personId)
    {
        const string sql = @"
            SELECT s.saksbehandler_id as SaksbehandlerId,
                   s.person_id as PersonId,
                   s.jobbepost as Jobbepost,
                   s.jobbtelefon as Jobbtelefon,
                   s.stilling as Stilling
            FROM saksbehandler s
            WHERE s.person_id = @PersonId";

        using var connection = _dbConnection.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SaksbehandlerModel>(sql, new { PersonId = personId });
    }

    public async Task<IEnumerable<SaksbehandlerNavnModel>> HentAlleSaksbehandlereNavnId()
    {
        using var connection = _dbConnection.CreateConnection();
        var sql = @"
        SELECT 
            s.saksbehandler_id as Id,
            CONCAT(p.fornavn, ' ', p.etternavn) as Navn
        FROM saksbehandler s
        JOIN person p ON s.person_id = p.person_id";

        return await connection.QueryAsync<SaksbehandlerNavnModel>(sql);
    }
}