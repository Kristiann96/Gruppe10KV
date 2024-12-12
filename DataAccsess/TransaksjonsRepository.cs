using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using Dapper;
using Interface;
using Models.Entities;
using Models.Exceptions;
using Models.Models;

namespace DataAccess
{
    public class TransaksjonsRepository : ITransaksjonsRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public TransaksjonsRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<bool> LagreKomplettInnmeldingAsync(
            int? gjestInnmelderId,
            InnmeldingModel innmelding,
            Geometri geometri)
        {
            using var connection = _dbConnection.CreateConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {

                innmelding.GjestInnmelderId = gjestInnmelderId;

                var innmeldingSql = @"
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

                var innmeldingId = await connection.ExecuteScalarAsync<int>(
                    innmeldingSql,
                    innmelding,
                    transaction);

                geometri.InnmeldingId = innmeldingId;
                var geometriSql = @"
                    INSERT INTO geometri (
                        innmelding_id,
                        geometri_data
                    ) VALUES (
                        @InnmeldingId,
                        ST_GeomFromGeoJSON(@GeometriGeoJson)
                    );";

                await connection.ExecuteAsync(
                    geometriSql,
                    geometri,
                    transaction);

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ForretningsRegelExceptionModel(
                    "Kunne ikke lagre innmeldingen: " + ex.Message,
                    ex);
            }
        }

        public void LagreKomplettInnmeldingAsync(string epost, InnmeldingModel innmelding, Geometri geometri)
        {
            throw new NotImplementedException();
        }

        public async Task<int> OpprettGjesteinnmelderAsync(string epost)
        {
            using var connection = _dbConnection.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var sql = @"
                INSERT INTO gjesteinnmelder (epost) 
                VALUES (@Epost);
                SELECT LAST_INSERT_ID();";

                var id = await connection.QuerySingleAsync<int>(sql,
                    new { Epost = epost },
                    transaction);

                await transaction.CommitAsync();
                return id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<(bool success, int personId)> OpprettPersonOgInnmelder(
            string fornavn,
            string etternavn,
            string telefonnummer,
            string epost)
        {
            using var connection = _dbConnection.CreateConnection();
            try
            {
                const string personSql = @"
                INSERT INTO person (fornavn, etternavn, telefonnummer)
                VALUES (@Fornavn, @Etternavn, @Telefonnummer);
                SELECT LAST_INSERT_ID();";

                var personId = await connection.QuerySingleAsync<int>(personSql,
                    new { Fornavn = fornavn, Etternavn = etternavn, Telefonnummer = telefonnummer });

                const string innmelderSql = @"
                INSERT INTO innmelder (person_id, innmelder_id, epost)
                VALUES (@PersonId, @InnmelderId, @Epost);";

                var innmelderId = await connection.QuerySingleAsync<int>(
                    "SELECT NEXTVAL(innmelder_id_seq)");

                await connection.ExecuteAsync(innmelderSql,
                    new { PersonId = personId, InnmelderId = innmelderId, Epost = epost });

                return (true, personId);
            }
            catch (Exception)
            {
                return (false, 0);

            }
        }

        public async Task<bool> SlettInnmeldingMedTilhorendeDataAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Slett geometri først (pga. fremmednøkkel)
                var geometriSql = "DELETE FROM geometri WHERE innmelding_id = @InnmeldingId";
                await connection.ExecuteAsync(geometriSql, new { InnmeldingId = innmeldingId }, transaction);

                // Så slett innmeldingen
                var innmeldingSql = "DELETE FROM innmelding WHERE innmelding_id = @InnmeldingId";
                var rowsAffected = await connection.ExecuteAsync(innmeldingSql, new { InnmeldingId = innmeldingId }, transaction);

                await transaction.CommitAsync();
                return rowsAffected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> LagreKomplettInnmeldingInnloggetAsync(
            string? epost,
            InnmeldingModel innmelding,
            Geometri geometri)
        {
            using var connection = _dbConnection.CreateConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var innmelderIdSql = @"
                    SELECT innmelder_id FROM innmelder WHERE epost = @Epost";

                var innmelderId = await connection.QuerySingleAsync<int>(
                    innmelderIdSql,
                    new { epost },
                    transaction);

                innmelding.InnmelderId = innmelderId;

                var innmeldingSql = @"
                    INSERT INTO innmelding (
                        innmelder_id, 
                        tittel, 
                        beskrivelse,
                        prioritet
                      
                    ) VALUES (
                        @InnmelderId,
                        @Tittel,
                        @Beskrivelse,
                        @Prioritet
                    );
                    SELECT LAST_INSERT_ID();";

                var innmeldingId = await connection.ExecuteScalarAsync<int>(
                    innmeldingSql,
                    innmelding,
                    transaction);

                geometri.InnmeldingId = innmeldingId;
                var geometriSql = @"
                    INSERT INTO geometri (
                        innmelding_id,
                        geometri_data
                    ) VALUES (
                        @InnmeldingId,
                        ST_GeomFromGeoJSON(@GeometriGeoJson)
                    );";

                await connection.ExecuteAsync(
                    geometriSql,
                    geometri,
                    transaction);

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ForretningsRegelExceptionModel(
                    "Kunne ikke lagre innmeldingen: " + ex.Message,
                    ex);
            }
        }
    }
}