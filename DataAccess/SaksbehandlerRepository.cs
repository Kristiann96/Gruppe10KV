using Dapper;
using Models;
using Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess
{
    public class SaksbehandlerRepository : ISaksbehandlerRepository
    {
        private readonly SaksbehandlerDBConnection _dbConnection;

        public SaksbehandlerRepository(SaksbehandlerDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<AdviserForm>> GetAllAdviserFormsAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            return await connection.QueryAsync<AdviserForm>("SELECT * FROM incident_form");
        }

        public async Task<AdviserForm> GetAdviserFormByIdAsync(int id)
        {
            using var connection = _dbConnection.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<AdviserForm>(
                "SELECT id, subject, uttrykning, something, attach_file, description, location_data AS GeoJson FROM incident_form WHERE id = @Id", new { Id = id });
        }

    }
}