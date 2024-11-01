using Dapper;
using MySqlConnector;
using System.Threading.Tasks;
using Models.Entities;
using Interfaces;


namespace DataAccess
{
    public class DataSammenstillingSaksBRepository : IDataSammenstillingSaksBRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public DataSammenstillingSaksBRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<(InnmeldingModel, PersonModel, InnmelderModel, SaksbehandlerModel)>
            GetInnmeldingMedDetaljerAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
            SELECT 
                -- Innmelding fields
                im.innmelding_id AS InnmeldingId, 
                im.tittel AS Tittel,
                im.beskrivelse AS Beskrivelse,
                im.siste_endring AS SisteEndring,
                im.status AS Status,
                im.prioritet AS Prioritet,
                im.kart_type AS KartType,
                -- Person fields
                p.person_id AS PersonId,
                p.fornavn AS Fornavn,
                p.etternavn AS Etternavn,
                p.telefonnummer AS Telefonnummer,
                -- Innmelder fields
                i.innmelder_id AS InnmelderId,
                i.innmelder_type AS InnmelderType,
                -- Saksbehandler fields
                s.saksbehandler_id AS SaksbehandlerId,
                s.stilling AS Stilling,
                s.jobbepost AS Jobbepost,
                s.jobbtelefon AS Jobbtelefon,
                -- Gjest fields
                g.gjest_innmelder_id AS GjestInnmelderId
            FROM innmelding im
            LEFT JOIN innmelder i ON im.innmelder_id = i.innmelder_id
            LEFT JOIN person p ON i.person_id = p.person_id
            LEFT JOIN saksbehandler s ON im.saksbehandler_id = s.saksbehandler_id
            LEFT JOIN gjesteinnmelder g ON im.gjest_innmelder_id = g.gjest_innmelder_id
            WHERE im.innmelding_id = @InnmeldingId";

            var result = await connection.QueryAsync<InnmeldingModel, PersonModel, InnmelderModel, SaksbehandlerModel,
                (InnmeldingModel, PersonModel, InnmelderModel, SaksbehandlerModel)>(
                sql,
                (innmelding, person, innmelder, saksbehandler) =>
                    (innmelding, person, innmelder, saksbehandler),
                new { InnmeldingId = innmeldingId },
                splitOn: "PersonId,InnmelderId,SaksbehandlerId"
            );

            return result.FirstOrDefault();
        }
    }
}
