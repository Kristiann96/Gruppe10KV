using Dapper;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interface;
using Models.Models;
using Interfaces;

namespace DataAccess
{
    public class GeometriRepository : IGeometriRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public GeometriRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }


        // Hent alle geometriobjekter for kartvisning KartvisningAlleInnmeldingerSaksB

        public async Task<IEnumerable<Geometri>> GetAllGeometriAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = "SELECT geometri_id AS GeometriId, innmelding_id AS InnmeldingId, ST_AsGeoJSON(geometri_data) AS GeometriGeoJson FROM geometri;";
            return await connection.QueryAsync<Geometri>(sql);
        }

        // Hent spesifikk geometri basert på innmelding_id  KartvisningEnInnmeldingSaksB
        public async Task<Geometri> GetGeometriByInnmeldingIdAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = "SELECT geometri_id, innmelding_id, ST_AsGeoJSON(geometri_data) AS GeometriGeoJson FROM geometri WHERE innmelding_id = @InnmeldingId";
            return await connection.QuerySingleOrDefaultAsync<Geometri>(sql, new { InnmeldingId = innmeldingId });
        }


        //LAGRING AV DATA

        public async Task<bool> LagreGeometriAsync(Geometri geometri)
        {
            using var connection = _dbConnection.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var sql = @"
                INSERT INTO geometri (
                    innmelding_id, 
                    geometri_data
                ) VALUES (
                    @InnmeldingId,
                    @GeometriGeoJson
                )";
                // Her er GeometriGeoJson allerede konvertert til WKT med SRID i Logic laget - DETTE BURDE GJØRES VIA LOGIC VED HENTING OGSÅ?!

                await connection.ExecuteAsync(sql, geometri, transaction);
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
