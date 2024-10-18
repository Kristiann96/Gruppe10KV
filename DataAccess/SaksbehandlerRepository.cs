﻿using Dapper;
using Interfaces;
using Models;
using Models.SaksbehandlerModels;
using Microsoft.Extensions.Logging;

namespace DataAccess
{
    public class SaksbehandlerRepository : ISaksbehandlerRepository
    {
        private readonly DapperDBConnection _dbConnection;
        private readonly ILogger<SaksbehandlerRepository> _logger;

        public SaksbehandlerRepository(DapperDBConnection dbConnection, ILogger<SaksbehandlerRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        // Metoder for incident_form tabellen
        public async Task<IEnumerable<SaksbehandlerInnmeldingModel>> GetAllAdviserFormsAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            return await connection.QueryAsync<SaksbehandlerInnmeldingModel>("SELECT * FROM incident_form");
        }

        public async Task<SaksbehandlerInnmeldingModel> GetAdviserFormByIdAsync(int id)
        {
            using var connection = _dbConnection.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<SaksbehandlerInnmeldingModel>(
                "SELECT id, subject, uttrykning, something, attach_file, description, location_data AS GeoJson FROM incident_form WHERE id = @Id",
                new { Id = id });
        }

        // Metoder for INNMELDING tabellen
        public async Task<IEnumerable<SaksbehandlerINNMELDINGModel>> GetAllInnmeldingerAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = "SELECT * FROM INNMELDING";
            return await connection.QueryAsync<SaksbehandlerINNMELDINGModel>(sql);
        }

        public async Task<SaksbehandlerINNMELDINGModel> GetInnmeldingByIdAsync(int innmeldID)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = "SELECT * FROM INNMELDING WHERE InnmeldID = @InnmeldID";
            return await connection.QuerySingleOrDefaultAsync<SaksbehandlerINNMELDINGModel>(sql, new { InnmeldID = innmeldID });
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
                var result = await connection.ExecuteAsync(sql, new { StatusID = status, InnmeldID = innmeldID }, transaction: transaction);

                await transaction.CommitAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Feil ved oppdatering av status for InnmeldID: {innmeldID}");
                return false;
            }
        }

        public async Task<bool> UpdateInnmeldingAsync(SaksbehandlerINNMELDINGModel innmelding)
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
                innmelding.StatusID,
                innmelding.SaksbehandlerID,
                SisteEndring = DateTime.Now,
                innmelding.PrioritetID,
                innmelding.InnmeldID
            });

            return result > 0;
        }

        public IEnumerable<StatusEnum> GetAvailableStatuses()
        {
            return Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>();
        }
    }
}