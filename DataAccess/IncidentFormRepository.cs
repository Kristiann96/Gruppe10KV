using Dapper;
using Interfaces;
using Models.Entities;

namespace DataAccess;

public class IncidentFormRepository : IIncidentFormRepository
{
    private readonly DapperDBConnection _dbConnection;

    public IncidentFormRepository(DapperDBConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<IncidentFormModel>> GetAllIncidentsAsync()
    {
        using var connection = _dbConnnection.CreateConnection();
        return await connection.QueryAsync<IncidentFormModel>("SELECT * FROM incident_form");
    }

    public async Task<IncidentFormModel> GetIncidentByIdAsync(int id)
    {
        using var connection = _dbConnnection.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<InnmelderIncidentFormModel>(
            "SELECT * FROM incident_form WHERE id = @Id", new { Id = id });
    }

    // Legger til transaksjon for innsending av skjemaet - innmelder
    public async Task<bool> SaveIncidentFormAsync(IncidentFormModel form)
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




    //Saksbehandler

        public async Task<IEnumerable<IncidentFormModel>> GetAllAdviserFormsAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            return await connection.QueryAsync<IncidentFormModel>("SELECT * FROM incident_form");
        }

        public async Task<IncidentFormModel> GetAdviserFormByIdAsync(int id)
        {
            using var connection = _dbConnection.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<IncidentFormModel>(
                "SELECT id, subject, uttrykning, something, attach_file, description, location_data AS GeoJson FROM incident_form WHERE id = @Id",
                new { Id = id });
        }
    }
}


