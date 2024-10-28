using Dapper;
using MySqlConnector;
using System.Threading.Tasks;
using Models.Entities;
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

        public async Task<Innmelder> GetInnmelderByIdAsync(int innmelderId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"SELECT i.*, p.fornavn, p.etternavn 
                        FROM innmelder i
                        JOIN person p ON i.person_id = p.person_id
                        WHERE i.innmelder_id = @InnmelderId";
            return await connection.QuerySingleOrDefaultAsync<Innmelder>(sql, new { InnmelderId = innmelderId });
        }
    }
}