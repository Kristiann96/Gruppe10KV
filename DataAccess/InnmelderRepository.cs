using Dapper;
using Interfaces;
using Models;
using Models.Entities;

namespace DataAccess;

public class InnmelderRepository : IInnmelderRepository
{
    private readonly DapperDBConnection _dbConnnection;

    public InnmelderRepository(DapperDBConnection dbConnnection)
    {
        _dbConnnection = dbConnnection;
    }

    public async Task<IEnumerable<InnmelderIncidentFormModel>> GetAllIncidentsAsync()
    {
        using var connection = _dbConnnection.CreateConnection();
        return await connection.QueryAsync<InnmelderIncidentFormModel>("SELECT * FROM incident_form");
    }

    public async Task<InnmelderIncidentFormModel> GetIncidentByIdAsync(int id)
    {
        using var connection = _dbConnnection.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<InnmelderIncidentFormModel>(
            "SELECT * FROM incident_form WHERE id = @Id", new { Id = id });
    }

    // Legger til transaksjon for innsending av skjemaet
    public async Task<bool> SaveIncidentFormAsync(InnmelderIncidentFormModel form)
    {
        using var connection = _dbConnnection.CreateConnection();
        await connection.OpenAsync(); // Åpner forbindelsen eksplisitt

        using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var sql =
                @"INSERT INTO incident_form (subject, uttrykning, something, attach_file, description, location_data) 
                            VALUES (@Subject, @Uttrykning, @Something, @AttachFile, @Description, @GeoJson)";

            // Utfør databaseoperasjonen med transaksjon
            await connection.ExecuteAsync(sql, form, transaction);

            // Fullfør transaksjonen hvis alt går bra
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            // Rull tilbake transaksjonen hvis det skjer en feil
            await transaction.RollbackAsync();
            return false;
        }
    }
}