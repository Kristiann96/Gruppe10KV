using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;

namespace DataAccess
{
    public class InnmeldingRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public InnmeldingRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Innmelding> GetInnmeldingByIdAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = "SELECT * FROM innmelding WHERE innmelding_id = @InnmeldingId";
            return await connection.QuerySingleOrDefaultAsync<Innmelding>(sql, new { InnmeldingId = innmeldingId });
        }
    }

}
