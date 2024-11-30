using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Interface;
using System.Security.Claims;


namespace DataAccess
{
    public class InnmeldingRepository : IInnmeldingRepository
    {
        private readonly DapperDBConnection _dbConnection;
        private const string ENUM_PREFIX = "enum(";
        private const string ENUM_SUFFIX = ")";

        public InnmeldingRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<InnmeldingModel>> GetInnmeldingAsync(int innmeldingIdUpdate)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"SELECT 
                    innmelding_id AS InnmeldingId,
                    tittel AS Tittel,
                    status AS Status,
                    beskrivelse AS Beskrivelse
                FROM innmelding
                WHERE innmelding_id = @InnmeldingId";

            return await connection.QueryAsync<InnmeldingModel>(sql, new { InnmeldingId = innmeldingIdUpdate });
        }


        public async Task<IEnumerable<InnmeldingModel>> HentInnmeldingerFraBrukerAsync(ClaimsPrincipal bruker)
        {
            var epost = bruker?.Identity?.Name;
            if (string.IsNullOrEmpty(epost))
                throw new InvalidOperationException("Brukerens e-post er ikke tilgjengelig.");

            using var connection = _dbConnection.CreateConnection();

            var sql = @"SELECT innmelding_id AS InnmeldingId,
                       tittel AS Tittel,
                       status AS Status,
                       siste_endring AS SisteEndring,
                       ig.innmelder_id AS InnmelderId
                FROM innmelding ig
                JOIN innmelder ir ON ig.innmelder_id = ir.innmelder_id
                WHERE ir.epost = @Epost";

            return await connection.QueryAsync<InnmeldingModel>(sql, new { Epost = epost });
        }


        private async Task<string> GetEnumValuesForColumnAsync(string tableName, string columnName)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
            SELECT SUBSTRING(COLUMN_TYPE, 
                           LENGTH(@EnumPrefix) + 1, 
                           LENGTH(COLUMN_TYPE) - LENGTH(@EnumPrefix) - LENGTH(@EnumSuffix)) AS EnumValues
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
            AND TABLE_NAME = @TableName
            AND COLUMN_NAME = @ColumnName";

            var parameters = new
            {
                TableName = tableName,
                ColumnName = columnName,
                EnumPrefix = ENUM_PREFIX,
                EnumSuffix = ENUM_SUFFIX
            };

            return await connection.QuerySingleOrDefaultAsync<string>(sql, parameters);
        }

        public async Task<string> GetStatusEnumValuesAsync() =>
            await GetEnumValuesForColumnAsync("innmelding", "status");

        public async Task<string> GetPrioritetEnumValuesAsync() =>
            await GetEnumValuesForColumnAsync("innmelding", "prioritet");

        public async Task<string> GetKartTypeEnumValuesAsync() =>
            await GetEnumValuesForColumnAsync("innmelding", "kart_type");

        // Todo: Flytt til eget repository
        public async Task<string> GetInnmelderTypeEnumValuesAsync() =>
            await GetEnumValuesForColumnAsync("innmelder", "innmelder_type");

        public async Task<bool> OppdatereEnumSaksBAsync(int innmeldingId, InnmeldingModel model)
        {
            using var connection = _dbConnection.CreateConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var sql = @"
                UPDATE innmelding 
                SET status = @Status,
                    prioritet = @Prioritet,
                    kart_type = @KartType
                WHERE innmelding_id = @InnmeldingId";

                var parameters = new
                {
                    InnmeldingId = innmeldingId,
                    Status = model.Status,
                    Prioritet = model.Prioritet,
                    KartType = model.KartType
                };

                var rowsAffected = await connection.ExecuteAsync(sql, parameters, transaction);
                await transaction.CommitAsync();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> OppdaterSaksbehandler(int innmeldingId, int? saksbehandlerId)
        {
            using var connection = _dbConnection.CreateConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var sql = @"
            UPDATE innmelding 
            SET saksbehandler_id = @SaksbehandlerId,
                siste_endring = CURRENT_TIMESTAMP
            WHERE innmelding_id = @InnmeldingId";

                var parameters = new
                {
                    InnmeldingId = innmeldingId,
                    SaksbehandlerId = saksbehandlerId
                };

                var rowsAffected = await connection.ExecuteAsync(sql, parameters, transaction);
                await transaction.CommitAsync();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> OppdaterInnmelderType(int innmelderId, InnmeldingModel model)
        {
            using var connection = _dbConnection.CreateConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var sql = @"
            UPDATE innmelder 
            SET innmelder_type = @InnmelderType
            WHERE innmelder_id = @InnmelderId";

                var parameters = new
                {
                    InnmelderId = innmelderId,
                    InnmelderType = model.InnmelderType
                };

                var rowsAffected = await connection.ExecuteAsync(sql, parameters, transaction);
                await transaction.CommitAsync();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> OppdatereTittelOgBeskrivelsePaaInnmeldingAsync(InnmeldingModel innmelding)
        {
            using var connection = _dbConnection.CreateConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var sql = @"
            UPDATE innmelding 
            SET tittel = @Tittel,
                beskrivelse = @Beskrivelse,
                siste_endring = CURRENT_TIMESTAMP
            WHERE innmelding_id = @InnmeldingId";

                var parameters = new
                {
                    innmelding.InnmeldingId,
                    innmelding.Tittel,
                    innmelding.Beskrivelse
                };

                var rowsAffected = await connection.ExecuteAsync(sql, parameters, transaction);
                await transaction.CommitAsync();

                return rowsAffected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<InnmeldingModel> HentInnmeldingOppsummeringAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
            SELECT 
                innmelding_id AS InnmeldingId,
                tittel AS Tittel, 
                status AS Status, 
                prioritet AS Prioritet,
                kart_type AS KartType
            FROM innmelding
            WHERE innmelding_id = @innmeldingId";

            return await connection.QuerySingleOrDefaultAsync<InnmeldingModel>(sql, new { InnmeldingId = innmeldingId });
        }
    }




}

