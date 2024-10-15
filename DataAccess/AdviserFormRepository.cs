using Dapper;
using Entities;
using Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess
{
    public class AdviserFormRepository : IAdviserFormRepository
    {
        private readonly AdviserFormDBContext _dbContext;

        public AdviserFormRepository(AdviserFormDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AdviserForm>> GetAllAdviserFormsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            return await connection.QueryAsync<AdviserForm>("SELECT * FROM incident_form");
        }

        public async Task<AdviserForm> GetAdviserFormByIdAsync(int id)
        {
            using var connection = _dbContext.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<AdviserForm>(
                "SELECT id, subject, uttrykning, something, attach_file, description, location_data AS GeoJson FROM incident_form WHERE id = @Id", new { Id = id });
        }

    }
}