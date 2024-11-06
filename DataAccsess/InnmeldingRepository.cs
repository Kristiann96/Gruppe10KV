using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Interface;


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



        //Daniel's sql innhenting av data til "OppdatereInnmelding"
        public async Task<IEnumerable<InnmeldingModel>> GetInnmeldingAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"SELECT innmelding_id AS InnmeldingId,
                    tittel AS Tittel,
                    status AS Status,
                    beskrivelse AS Beskrivelse
                FROM innmelding
                WHERE innmelding_id = 8";
            var result = await connection.QueryAsync<InnmeldingModel>(sql);

            return result;
        }

        /* Ørjan */
        public async Task<IEnumerable<InnmeldingModel>> GetOversiktAlleInnmeldingerSaksBAsync(int pageNumber,
            int pageSize, string searchTerm)
        {
            using var connection = _dbConnection.CreateConnection();

            var sql = @"SELECT innmelding_id AS InnmeldingId,
                            innmelder_id AS InnmelderId,
                            tittel AS Tittel,
                            status AS Status,

                            siste_endring AS SisteEndring,
                            prioritet AS Prioritet
                        FROM innmelding
                        WHERE tittel LIKE @SearchTerm
                        ORDER BY innmelding_id
                        LIMIT @PageSize OFFSET @Offset";

            var parameters = new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize,
                SearchTerm = "%" + searchTerm + "%"
            };

            return await connection.QueryAsync<InnmeldingModel>(sql, parameters);
        }

        public async Task<int> GetTotalInnmeldingerTellerSaksBAsync(string searchTerm)
        {
            using var connection = _dbConnection.CreateConnection();

            var sql = @"SELECT COUNT(*)
                        FROM innmelding
                        WHERE tittel LIKE @SearchTerm";

            var parameters = new
            {
                SearchTerm = "%" + searchTerm + "%"
            };

            return await connection.ExecuteScalarAsync<int>(sql, parameters);

        }

        /* Ørjan over */

        //InnmeldingEnumLogic for status
        /*public async Task<string> GetStatusEnumValuesAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
            SELECT SUBSTRING(COLUMN_TYPE, 6, LENGTH(COLUMN_TYPE) - 6) AS EnumValues
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
            AND TABLE_NAME = 'innmelding'
            AND COLUMN_NAME = 'status'";
            return await connection.QuerySingleOrDefaultAsync<string>(sql);
        }*/


        public async Task<IEnumerable<InnmeldingModel>> HentInnmeldingerFraInnmelderIdAsync(int innmelderId)
        {
            using var connection = _dbConnection.CreateConnection();

            var sql = @"SELECT innmelding_id AS InnmeldingId,
                       tittel AS Tittel,
                       status AS Status,
                       siste_endring AS SisteEndring,
                       innmelder_id AS InnmelderId
                FROM innmelding
                WHERE innmelder_id = @InnmelderId";

            return await connection.QueryAsync<InnmeldingModel>(sql, new { InnmelderId = innmelderId });
        }

        //Henting for enummene

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


        //Todo: Flytt til eget repository (eller slette?)
        public async Task<string> GetInnmelderTypeEnumValuesAsync() =>
            await GetEnumValuesForColumnAsync("innmelder", "innmelder_type");

        //Oppdatering av enum
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

    }
}

