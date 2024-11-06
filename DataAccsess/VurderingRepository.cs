using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interface;
using Models.Entities;

namespace DataAccess
{
    public class VurderingRepository : IVurderingRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public VurderingRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<(int antallBekreftelser, int antallAvkreftelser)> HentAntallVurderingerAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
                SELECT 
                    SUM(CASE WHEN type = 'bekreftelse' THEN 1 ELSE 0 END) as antallBekreftelser,
                    SUM(CASE WHEN type = 'avkreftelse' THEN 1 ELSE 0 END) as antallAvkreftelser
                FROM vurdering 
                WHERE innmelding_id = @InnmeldingId";

            var result = await connection.QueryFirstAsync<(int, int)>(sql, new { InnmeldingId = innmeldingId });
            return result;
        }

        public async Task<IEnumerable<VurderingModel>> HentVurderingerForInnmeldingAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
                SELECT 
                    vurdering_id as VurderingId,
                    innmelding_id as InnmeldingId,
                    innmelder_id as InnmelderId,
                    type as Type,
                    kommentar as Kommentar,
                    dato as Dato
                FROM vurdering 
                WHERE innmelding_id = @InnmeldingId
                ORDER BY dato DESC";

            return await connection.QueryAsync<VurderingModel>(sql, new { InnmeldingId = innmeldingId });
        }

        public async Task<int> LeggTilVurderingAsync(VurderingModel vurdering)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
                INSERT INTO vurdering (
                    innmelding_id, 
                    innmelder_id,
                    type,
                    kommentar
                ) VALUES (
                    @InnmeldingId,
                    @InnmelderId,
                    @Type,
                    @Kommentar
                );
                SELECT LAST_INSERT_ID();";

            return await connection.ExecuteScalarAsync<int>(sql, vurdering);
        }

        public async Task<IEnumerable<string>> HentKommentarerForInnmeldingAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
                SELECT kommentar 
                FROM vurdering 
                WHERE innmelding_id = @InnmeldingId 
                AND kommentar IS NOT NULL 
                ORDER BY dato DESC";

            return await connection.QueryAsync<string>(sql, new { InnmeldingId = innmeldingId });
        }
    }
}
