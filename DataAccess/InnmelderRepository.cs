using Dapper;
using Models;
using Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess
{
    public class InnmelderRepository : IInnmelderRepository
    {
        private readonly InnmelderDBConnection _dbConnnection;

        public InnmelderRepository(InnmelderDBConnection dbConnnection)
        {
            _dbConnnection = dbConnnection;
        }

        public async Task<IEnumerable<IncidentForm>> GetAllIncidentsAsync()
        {
            using var connection = _dbConnnection.CreateConnection();
            return await connection.QueryAsync<IncidentForm>("SELECT * FROM incident_form");
        }

        public async Task<IncidentForm> GetIncidentByIdAsync(int id)
        {
            using var connection = _dbConnnection.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<IncidentForm>(
                "SELECT * FROM incident_form WHERE id = @Id", new { Id = id });
        }

        public async Task SaveIncidentFormAsync(IncidentForm form)
        {
            using var connection = _dbConnnection.CreateConnection();
            var sql = @"INSERT INTO incident_form (subject, uttrykning, something, attach_file, description, location_data) 
                        VALUES (@Subject, @Uttrykning, @Something, @AttachFile, @Description, @GeoJson)";
            await connection.ExecuteAsync(sql, form);
        }
    }
}