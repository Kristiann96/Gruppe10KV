using Dapper;
using Entities;
using Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess
{
    public class IncidentFormRepository : IIncidentFormRepository
    {
        private readonly IncidentFormDBContext _dbContext;

        public IncidentFormRepository(IncidentFormDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IncidentForm>> GetAllIncidentsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            return await connection.QueryAsync<IncidentForm>("SELECT * FROM incident_form");
        }

        public async Task<IncidentForm> GetIncidentByIdAsync(int id)
        {
            using var connection = _dbContext.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<IncidentForm>(
                "SELECT * FROM incident_form WHERE id = @Id", new { Id = id });
        }

        public async Task SaveIncidentFormAsync(IncidentForm form)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"INSERT INTO incident_form (subject, uttrykning, something, attach_file, description, location_data) 
                        VALUES (@Subject, @Uttrykning, @Something, @AttachFile, @Description, @GeoJson)";
            await connection.ExecuteAsync(sql, form);
        }
    }
}