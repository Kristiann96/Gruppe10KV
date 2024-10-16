using Dapper;
using Models;
using Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess
{
    public class InnmelderRepository : IInnmelderRepository
    {
        private readonly DapperDBConnection _dbConnnection;

        public InnmelderRepository(DapperDBConnection dbConnnection)
        {
            _dbConnnection = dbConnnection;
        }

        public async Task<IEnumerable<InnmelderSkjemaModel>> GetAllIncidentsAsync()
        {
            using var connection = _dbConnnection.CreateConnection();
            return await connection.QueryAsync<InnmelderSkjemaModel>("SELECT * FROM incident_form");
        }

        public async Task<InnmelderSkjemaModel> GetIncidentByIdAsync(int id)
        {
            using var connection = _dbConnnection.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<InnmelderSkjemaModel>(
                "SELECT * FROM incident_form WHERE id = @Id", new { Id = id });
        }

        public async Task SaveIncidentFormAsync(InnmelderSkjemaModel form)
        {
            using var connection = _dbConnnection.CreateConnection();
            var sql = @"INSERT INTO incident_form (subject, uttrykning, something, attach_file, description, location_data) 
                        VALUES (@Subject, @Uttrykning, @Something, @AttachFile, @Description, @GeoJson)";
            await connection.ExecuteAsync(sql, form);
        }
    }
}