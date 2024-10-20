using Dapper;
using Interfaces;
using Models.Entities;

using Microsoft.Extensions.Logging;

namespace DataAccess;


    public class SaksbehandlerRepository : ISaksbehandlerRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public SaksbehandlerRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<SaksbehandlerIncidentFormModel>> GetAllAdviserFormsAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            return await connection.QueryAsync<SaksbehandlerIncidentFormModel>("SELECT * FROM incident_form");
        }

        public async Task<SaksbehandlerIncidentFormModel> GetAdviserFormByIdAsync(int id)
        {
            using var connection = _dbConnection.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<SaksbehandlerIncidentFormModel>(
                "SELECT id, subject, uttrykning, something, attach_file, description, location_data AS GeoJson FROM incident_form WHERE id = @Id",
                new { Id = id });
        }

        // Metoder for INNMELDING-tabellen
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

        // Oppdaterer kun status i INNMELDING-tabellen
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

        // Oppdaterer hele innmeldingen (alle felter)
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

        // Henter alle tilgjengelige statuser (til bruk i dropdown)
        public IEnumerable<StatusEnum> GetAvailableStatuses()
        {
            return Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>();
        }
    }




