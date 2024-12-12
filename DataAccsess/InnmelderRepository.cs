using Dapper;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using Interface;

namespace DataAccess
{
    public class InnmelderRepository : IInnmelderRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public InnmelderRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<InnmelderModel?> HentInnmelderTypeAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
            SELECT 
                i.innmelder_id AS InnmelderId,
                i.innmelder_type AS InnmelderType
            FROM innmelding im
            JOIN innmelder i ON im.innmelder_id = i.innmelder_id
            WHERE im.innmelding_id = @innmeldingId";

            return await connection.QuerySingleOrDefaultAsync<InnmelderModel>(sql, new { InnmeldingId = innmeldingId });
        }
    }
}
