using Dapper;
using Models;
using Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess
{
    public class SaksbehandlerRepository : ISaksbehandlerRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public SaksbehandlerRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<SaksbehandlerInnmeldingModel>> GetAllAdviserFormsAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            return await connection.QueryAsync<SaksbehandlerInnmeldingModel>("SELECT * FROM incident_form");
        }

        public async Task<SaksbehandlerInnmeldingModel> GetAdviserFormByIdAsync(int id)
        {
            using var connection = _dbConnection.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<SaksbehandlerInnmeldingModel>(
                "SELECT id, subject, uttrykning, something, attach_file, description, location_data AS GeoJson FROM incident_form WHERE id = @Id", new { Id = id });
        }

    }
}