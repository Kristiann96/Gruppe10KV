using Dapper;
using Interfaces;
using Models.Entities;

namespace DataAccess;

public class InnmeldingeRepository : IInnmeldingERepository
{
    private readonly DapperDBConnection _dbConnection;

    public InnmeldingERepository(DapperDBConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<InnmeldingEModel>> GetAllInnmeldingerAsync()
    {
        using var connection = _dbConnection.CreateConnection();
        var sql = "SELECT * FROM INNMELDING";
        return await connection.QueryAsync<InnmeldingEModel>(sql);
    }

    public async Task<InnmeldingEModel> GetInnmeldingByIdAsync(int innmeldID)
    {
        using var connection = _dbConnection.CreateConnection();
        var sql = "SELECT * FROM INNMELDING WHERE InnmeldID = @InnmeldID";
        return await connection.QuerySingleOrDefaultAsync<InnmeldingEModel>(sql,
            new { InnmeldID = innmeldID });
    }

    public async Task<StatusEnum> GetStatusByInnmeldIdAsync(int innmeldID)
    {
        using var connection = _dbConnection.CreateConnection();
        var sql = "SELECT StatusID FROM INNMELDING WHERE InnmeldID = @InnmeldID";
        return await connection.QuerySingleOrDefaultAsync<StatusEnum>(sql, new { InnmeldID = innmeldID });
    }

    public async Task<bool> UpdateStatusAsync(int innmeldID, StatusEnum status)
    {
        using var connection = _dbConnection.CreateConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var sql = "UPDATE INNMELDING SET StatusID = @StatusID WHERE InnmeldID = @InnmeldID";
            var result = await connection.ExecuteAsync(sql, new { StatusID = status, InnmeldID = innmeldID },
                transaction: transaction);

            await transaction.CommitAsync();
            return result > 0;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Feil ved oppdatering av status for InnmeldID: {innmeldID}, Exception: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateInnmeldingAsync(InnmeldingEModel innmeldingE)
    {
        using var connection = _dbConnection.CreateConnection();
        var sql = @"UPDATE INNMELDING 
                        SET StatusID = @StatusID, 
                            SaksbehandlerID = @SaksbehandlerID, 
                            SisteEndring = @SisteEndring, 
                            PrioritetID = @PrioritetID, 
                            InnmelderID = @InnmelderID
                        WHERE InnmeldID = @InnmeldID";

        var result = await connection.ExecuteAsync(sql, new
        {
            innmeldingE.StatusID,
            innmeldingE.SaksbehandlerID,
            SisteEndring = DateTime.Now,
            innmeldingE.PrioritetID,
            innmeldingE.InnmeldID,
            innmeldingE.InnmelderID
        });

        return result > 0;
    }

    public IEnumerable<StatusEnum> GetAvailableStatuses()
    {
        return Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>();
    }
}
