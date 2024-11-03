using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                string gjesteEpost,
                InnmeldingModel innmelding,
                Geometri geometri)
            {
                using var connection = _dbConnection.CreateConnection();
                using var transaction = await connection.BeginTransactionAsync();

                try
                {
                    // 1. Opprett gjesteinnmelder
                    var gjesteInnmelderSql = @"
                    INSERT INTO gjesteinnmelder (epost) 
                    VALUES (@Epost);
                    SELECT LAST_INSERT_ID();";

                    var gjestInnmelderId = await connection.ExecuteScalarAsync<int>(
                        gjesteInnmelderSql,
                        new { Epost = gjesteEpost },
                        transaction);

                    // 2. Opprett innmelding
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

                    // 3. Lagre geometri
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
