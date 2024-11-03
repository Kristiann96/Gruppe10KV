﻿using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Interface;
using Models.DTOs;

namespace DataAccess
{
    public class InnmeldingRepository : IInnmeldingRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public InnmeldingRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        //HENTNING AV DATA

        //dto -skal ikke brukes - skal refactores bort
        public async Task<InnmeldingModel> GetInnmeldingByIdAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
        SELECT im.innmelding_id AS InnmeldingId, im.tittel AS Tittel, im.beskrivelse AS Beskrivelse,
               im.siste_endring AS SisteEndring, im.status AS Status, im.prioritet AS Prioritet, im.kart_type AS KartType,
               i.innmelder_id AS InnmelderId, i.innmelder_type AS InnmelderType,
               p.fornavn AS Fornavn, p.etternavn AS Etternavn,
               s.saksbehandler_id AS SaksbehandlerId, s.stilling AS SaksbehandlerStilling,
               g.gjest_innmelder_id AS GjestInnmelderId
        FROM innmelding im
        LEFT JOIN innmelder i ON im.innmelder_id = i.innmelder_id
        LEFT JOIN person p ON i.person_id = p.person_id
        LEFT JOIN saksbehandler s ON im.saksbehandler_id = s.saksbehandler_id
        LEFT JOIN gjesteinnmelder g ON im.gjest_innmelder_id = g.gjest_innmelder_id
        WHERE im.innmelding_id = @InnmeldingId";

            return await connection.QuerySingleOrDefaultAsync<InnmeldingModel>(sql,
                new { InnmeldingId = innmeldingId });
        }

        // KartvisningEnInnmelding
        public async Task<InnmeldingDetaljerKartvisningSaksBModel> GetInnmeldingDetaljerByIdAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
        SELECT im.innmelding_id AS InnmeldingId, im.tittel AS Tittel, im.beskrivelse AS Beskrivelse,
               im.siste_endring AS SisteEndring, im.status AS Status,im.prioritet AS Prioritet, im.kart_type AS KartType,
               i.innmelder_id AS InnmelderId, i.innmelder_type AS InnmelderType,
               p.fornavn AS Fornavn, p.etternavn AS Etternavn,
               s.saksbehandler_id AS SaksbehandlerId, s.stilling AS SaksbehandlerStilling,
               g.gjest_innmelder_id AS GjestInnmelderId
        FROM innmelding im
        LEFT JOIN innmelder i ON im.innmelder_id = i.innmelder_id
        LEFT JOIN person p ON i.person_id = p.person_id
        LEFT JOIN saksbehandler s ON im.saksbehandler_id = s.saksbehandler_id
        LEFT JOIN gjesteinnmelder g ON im.gjest_innmelder_id = g.gjest_innmelder_id
        WHERE im.innmelding_id = @InnmeldingId";

            return await connection.QuerySingleOrDefaultAsync<InnmeldingDetaljerKartvisningSaksBModel>(sql,
                new { InnmeldingId = innmeldingId });
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

        //InnmeldingEnumLogic
        public async Task<string> GetStatusEnumValuesAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
            SELECT SUBSTRING(COLUMN_TYPE, 6, LENGTH(COLUMN_TYPE) - 6) AS EnumValues
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
            AND TABLE_NAME = 'innmelding'
            AND COLUMN_NAME = 'status'";
            return await connection.QuerySingleOrDefaultAsync<string>(sql);
        }


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


        //LAGRING AV DATA

        public async Task<int> LagreInnmeldingAsync(InnmeldingModel innmelding)
        {
            using var connection = _dbConnection.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var sql = @"
                INSERT INTO innmelding (
                    gjest_innmelder_id, 
                    tittel, 
                    beskrivelse, 
                    prioritet
                ) VALUES (
                    @GjestInnmelderId,
                    @Tittel,
                    @Beskrivelse,
                    @Prioritet   
                );
                SELECT LAST_INSERT_ID();";

                var id = await connection.QuerySingleAsync<int>(sql, innmelding, transaction);
                await transaction.CommitAsync();
                return id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }
    }
}

