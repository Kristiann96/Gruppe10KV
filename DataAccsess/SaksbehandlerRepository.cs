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
    public class SaksbehandlerRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public SaksbehandlerRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Saksbehandler> GetSaksbehandlerByIdAsync(int saksbehandlerId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"SELECT s.*, p.fornavn, p.etternavn 
                    FROM saksbehandler s
                    JOIN person p ON s.person_id = p.person_id
                    WHERE s.saksbehandler_id = @SaksbehandlerId";
            return await connection.QuerySingleOrDefaultAsync<Saksbehandler>(sql, new { SaksbehandlerId = saksbehandlerId });
        }
    }

}
