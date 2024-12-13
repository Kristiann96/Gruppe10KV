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

        public async Task<(bool success, int personId, string? errorMessage)> OpprettPersonOgInnmelder(
     string fornavn,
     string etternavn,
     string telefonnummer,
     string epost)
        {
            using var connection = _dbConnection.CreateConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                const string sjekkTelefonSql = "SELECT COUNT(*) FROM person WHERE telefonnummer = @Telefonnummer";
                var telefonFinnes = await connection.ExecuteScalarAsync<int>(
                    sjekkTelefonSql,
                    new { Telefonnummer = telefonnummer },
                    transaction) > 0;

                if (telefonFinnes)
                {
                    throw new ForretningsRegelExceptionModel("Telefonnummer er allerede i bruk");
                }

                const string sjekkEpostSql = "SELECT COUNT(*) FROM innmelder WHERE epost = @Epost";
                var epostFinnes = await connection.ExecuteScalarAsync<int>(
                    sjekkEpostSql,
                    new { Epost = epost },
                    transaction) > 0;

                if (epostFinnes)
                {
                    throw new ForretningsRegelExceptionModel("E-post er registrert, vennligst velg en annen");
                }

              
                const string personSql = @"
            INSERT INTO person (fornavn, etternavn, telefonnummer)
            VALUES (@Fornavn, @Etternavn, @Telefonnummer);
            SELECT LAST_INSERT_ID();";

                var personId = await connection.QuerySingleAsync<int>(personSql,
                    new { Fornavn = fornavn, Etternavn = etternavn, Telefonnummer = telefonnummer },
                    transaction);

                const string innmelderSql = @"
            INSERT INTO innmelder (person_id, innmelder_id, epost)
            VALUES (@PersonId, @InnmelderId, @Epost);";

                var innmelderId = await connection.QuerySingleAsync<int>(
                    "SELECT NEXTVAL(innmelder_id_seq)",
                    transaction: transaction);

                await connection.ExecuteAsync(innmelderSql,
                    new { PersonId = personId, InnmelderId = innmelderId, Epost = epost },
                    transaction);

                await transaction.CommitAsync();
                return (true, personId, null);
            }
            catch (ForretningsRegelExceptionModel ex)
            {
                await transaction.RollbackAsync();
                return (false, 0, ex.Message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, 0, "Det oppstod en teknisk feil under registreringen. Vennligst prøv igjen senere.");
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