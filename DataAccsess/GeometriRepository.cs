using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
///using static DataAccess.GeometriRepository;



//MOCK METODE/KLASSE FOR EKSEMPEL - SANSYNLIGVIS HALVVEIS BRUKBART
/*namespace DataAccess
{
    internal class GeometriRepository
    {

        public class GeometriRepository : IGeometriRepository
        {
            private readonly IDbConnection _dbConnection;

            public GeometriRepository(IDbConnection dbConnection)
            {
                _dbConnection = dbConnection; //MÅ ENDRES -DA DISSE KOMMUNISERER VIE EGEN CONNETRIONSTRING TIL katverket databasen ikke Karteveket med stor K databasen
            }

            // Lagre GeoJSON som geometry-data i databasen
            public async Task AddGeometriAsync(int innmeldingId, string geoJson)
            {
                var sql = "INSERT INTO geometri (innmelding_id, geometri_data) VALUES (@InnmeldingId, ST_GeomFromGeoJSON(@GeoJsonData))";
                await _dbConnection.ExecuteAsync(sql, new { InnmeldingId = innmeldingId, GeoJsonData = geoJson });
            }

            // Hent geometry-data som GeoJSON fra databasen
            public async Task<string?> GetGeometriAsGeoJsonAsync(int geometriId)
            {
                var sql = "SELECT ST_AsGeoJSON(geometri_data) AS GeometriGeoJson FROM geometri WHERE geometri_id = @GeometriId";
                return await _dbConnection.QuerySingleOrDefaultAsync<string>(sql, new { GeometriId = geometriId });
            }
        }
    }
}*/
