using Dapper;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interface;
using Models.Models;

namespace DataAccess
{
    public class GeometriRepository : IGeometriRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public GeometriRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Hent alle geometriobjekter for kartvisning
        public async Task<IEnumerable<Geometri>> GetAllGeometriAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = "SELECT geometri_id, innmelding_id, ST_AsGeoJSON(geometri_data) AS GeometriGeoJson FROM geometri";
            return await connection.QueryAsync<Geometri>(sql);
        }

        // Hent spesifikk geometri basert på innmelding_id
        public async Task<Geometri> GetGeometriByInnmeldingIdAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = "SELECT geometri_id, innmelding_id, ST_AsGeoJSON(geometri_data) AS GeometriGeoJson FROM geometri WHERE innmelding_id = @InnmeldingId";
            return await connection.QuerySingleOrDefaultAsync<Geometri>(sql, new { InnmeldingId = innmeldingId });
        }
    }
}
