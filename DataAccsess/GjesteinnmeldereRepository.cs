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

        public async Task<int> OpprettGjesteinnmelderAsync(GjesteinnmelderModel gjesteinnmelder)
        {
            using var connection = _dbConnection.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var sql = @"
                INSERT INTO gjesteinnmelder (epost) 
                VALUES (@Epost);
                SELECT LAST_INSERT_ID();";

                var id = await connection.QuerySingleAsync<int>(sql,
                    new { Epost = gjesteinnmelder.Epost },
                    transaction);

                await transaction.CommitAsync();
                return id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public interface IGjesteinnmelderRepository
    {
        Task<int> OpprettGjesteinnmelderAsync(GjesteinnmelderModel gjesteinnmelder);
    }