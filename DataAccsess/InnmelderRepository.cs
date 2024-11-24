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

        public async Task<bool> ErGyldigInnmelderEpost(string epost)
        {
            const string sql = @"
            SELECT COUNT(1) 
            FROM innmelder i
            WHERE i.epost = @Epost";

            using var connection = _dbConnection.CreateConnection();
            var count = await connection.QuerySingleOrDefaultAsync<int>(sql, new { Epost = epost });
            return count > 0;
        }

        public async Task<InnmelderModel?> HentInnmelderMedEpost(string epost)
        {
            const string sql = @"
            SELECT i.innmelder_id as InnmelderId, 
                   i.person_id as PersonId,
                   i.epost as Epost,
                   i.innmelder_type as InnmelderType
            FROM innmelder i
            WHERE i.epost = @Epost";

            using var connection = _dbConnection.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<InnmelderModel>(sql, new { Epost = epost });
        }

        public async Task<InnmelderModel?> HentInnmelding(int personId)
        {
            const string sql = @"
            SELECT i.innmelder_id as InnmelderId, 
                   i.person_id as PersonId,
                   i.epost as Epost,
                   i.innmelder_type as InnmelderType
            FROM innmelder i
            WHERE i.person_id = @PersonId";

            using var connection = _dbConnection.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<InnmelderModel>(sql, new { PersonId = personId });
        }

        public async Task<int> HentInnmelderIdMedEpost(string epost)
        {
            const string sql = @"
            SELECT i.innmelder_id as InnmelderId
            FROM innmelder i
            WHERE i.epost = @Epost";

            using var connection = _dbConnection.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<int>(sql, new { Epost = epost });
        }
    }
}
