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

        public async Task<int?> HentGjesteinnmelderIdAsync(string? gjesteinnmelderEpost)
        {
            using var connection = _dbConnection.CreateConnection();

            var sql = @"
            SELECT gjest_innmelder_id as gjesteinnmelderId FROM gjesteinnmelder
            WHERE epost = @gjesteinnmelderEpost;";

            var gjesteinnmelderId = await connection.QuerySingleOrDefaultAsync<int?>(sql,
                new { gjesteinnmelderEpost = gjesteinnmelderEpost });

            return gjesteinnmelderId;
        }
    }
}