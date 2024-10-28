using Dapper;
using MySqlConnector;
using System.Threading.Tasks;
using Interface;
using Models.Entities;

namespace DataAccess
{
    public class GjesteinnmelderRepository : IGjesteinnmelderRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public GjesteinnmelderRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Gjesteinnmelder> GetGjesteinnmelderByIdAsync(int gjestInnmelderId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"SELECT * FROM gjesteinnmelder
                        WHERE gjest_innmelder_id = @GjestInnmelderId";
            return await connection.QuerySingleOrDefaultAsync<Gjesteinnmelder>(sql, new { GjestInnmelderId = gjestInnmelderId });
        }
    }
}