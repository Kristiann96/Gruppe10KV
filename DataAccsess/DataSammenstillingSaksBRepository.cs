using Dapper;
using MySqlConnector;
using System.Threading.Tasks;
using Models.Entities;
using Interfaces;
using Models.Models;

//brukes for spørringer som sammenstiller flere tabeller
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
                s.stilling AS SaksbehadlderStilling,
                s.jobbepost AS SaksbehandlerJobbepost,
                s.jobbtelefon AS SaksbehandlerJobbtelefon,
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

        public async
            Task<(IEnumerable<(InnmeldingModel, PersonModel, Geometri, GjesteinnmelderModel, InnmelderModel)> Data, int
                TotalPages)> GetOversiktAlleInnmeldingerSaksBAsync(int pageNumber, int pageSize, string searchTerm)
        {
            await using var connection = _dbConnection.CreateConnection();


            var countSql = @"
                SELECT COUNT(*)
                FROM innmelding im
                LEFT JOIN innmelder i ON im.innmelder_id = i.innmelder_id
                LEFT JOIN person p ON i.person_id = p.person_id
                LEFT JOIN geometri g ON im.innmelding_id = g.innmelding_id
                LEFT JOIN gjesteinnmelder gi ON im.gjest_innmelder_id = gi.gjest_innmelder_id
                WHERE im.tittel LIKE @SearchTerm
                OR CONCAT (p.fornavn, ' ', p.etternavn) LIKE @SearchTerm
                OR i.epost LIKE @SearchTerm
                OR gi.epost LIKE @SearchTerm";

            var totalItems =
                await connection.ExecuteScalarAsync<int>(countSql, new { SearchTerm = "%" + searchTerm + "%" });


            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);


            var dataSql = @"
                SELECT
                    -- Innmelding fields
                    im.innmelding_id AS InnmeldingId,
                    im.tittel AS Tittel,
                    im.beskrivelse AS Beskrivelse,
                    im.innmeldingstidspunkt AS Innmeldingstidspunkt,
                    im.siste_endring AS SisteEndring,
                    im.status AS Status,
                    im.prioritet AS Prioritet,
                    im.kart_type AS KartType,
                    im.innmelder_id,
                    im.saksbehandler_id,
                    im.gjest_innmelder_id,

                    -- Person fields
                    p.person_id AS PersonId,
                    p.fornavn AS Fornavn,
                    p.etternavn AS Etternavn,
                    p.telefonnummer AS Telefonnummer,

                    -- Geometri fields
                    g.geometri_id AS GeometriId,
                    g.innmelding_id AS InnmeldingId,
                    ST_AsText(g.geometri_data) AS GeometriGeoJson,

                    -- Gjesteinnmelder fields
                    COALESCE(gi.gjest_innmelder_id, 0) AS GjestInnmelderId,
                    COALESCE(gi.epost, '') AS Epost,

                    -- Innmelder fields
                    COALESCE(i.innmelder_id, 0) AS InnmelderId,
                    COALESCE(i.person_id, 0) AS PersonId,
                    COALESCE(i.epost, '') AS Epost,
                    COALESCE(i.innmelder_type, '') AS InnmelderType
                FROM innmelding im
                LEFT JOIN innmelder i ON im.innmelder_id = i.innmelder_id
                LEFT JOIN person p ON i.person_id = p.person_id
                LEFT JOIN geometri g ON im.innmelding_id = g.innmelding_id
                LEFT JOIN gjesteinnmelder gi ON im.gjest_innmelder_id = gi.gjest_innmelder_id
                WHERE im.tittel LIKE @SearchTerm
                OR CONCAT (p.fornavn, ' ', p.etternavn) LIKE @SearchTerm
                OR i.epost LIKE @SearchTerm
                OR gi.epost LIKE @SearchTerm
                ORDER BY 
                    FIELD(im.status, 'ny', 'ikke_på_begynt', 'under_behandling', 'pauset', 'avsluttet', 'ikke_tatt_til_følge'),
                    im.siste_endring DESC
                LIMIT @PageSize OFFSET @Offset";

            var parameters = new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize,
                SearchTerm = "%" + searchTerm + "%"
            };



            var result = await connection
                .QueryAsync<InnmeldingModel, PersonModel, Geometri, GjesteinnmelderModel, InnmelderModel,
                    (InnmeldingModel, PersonModel, Geometri, GjesteinnmelderModel, InnmelderModel)>(
                    dataSql,
                    (innmelding, person, geometri, gjesteinnmelder, innmelder) =>
                    {

                        if (gjesteinnmelder != null)
                        {
                            Console.WriteLine($"GjesteinnmelderEmail before: {gjesteinnmelder.Epost}");
                            gjesteinnmelder.Epost = string.IsNullOrEmpty(gjesteinnmelder.Epost)
                                ? "N/A"
                                : gjesteinnmelder.Epost;
                            Console.WriteLine($"GjesteinnmelderEmail after: {gjesteinnmelder.Epost}");
                        }


                        if (innmelder != null)
                        {
                            Console.WriteLine($"InnmelderEmail before: {innmelder.Epost}");
                            innmelder.Epost = string.IsNullOrEmpty(innmelder.Epost) ? "N/A" : innmelder.Epost;
                            Console.WriteLine($"InnmelderEmail after: {innmelder.Epost}");
                        }

                        return (innmelding, person, geometri, gjesteinnmelder, innmelder);
                    },
                    parameters,
                    splitOn: "PersonId,GeometriId,GjestInnmelderId,InnmelderId"
                );


            var resultList = result.ToList();

            return (resultList, totalPages);
        }


        public async Task<(InnmeldingModel, PersonModel?, InnmelderModel?, SaksbehandlerModel?)>
            ForBehandlingAvInnmeldingAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
        SELECT 
            -- Innmelding fields
            i.tittel AS Tittel,
            i.beskrivelse AS Beskrivelse,
            -- Person fields
            p.fornavn AS Fornavn,
            p.etternavn AS Etternavn,
            p.telefonnummer AS Telefonnummer,
            -- Innmelder fields
            im.innmelder_id AS InnmelderId,
            im.innmelder_type AS InnmelderType,
            -- Saksbehandler fields
            sb.stilling AS Stilling,
            sb.jobbepost AS Jobbepost,
            sb.jobbtelefon AS Jobbtelefon
            -- Gjest fields
            g.gjest_innmelder_id AS GjestInnmelderId
        FROM innmelding i
        LEFT JOIN innmelder im ON i.innmelder_id = im.innmelder_id
        LEFT JOIN person p ON im.person_id = p.person_id
        LEFT JOIN saksbehandler sb ON i.saksbehandler_id = sb.saksbehandler_id
        LEFT JOIN gjesteinnmelder g ON i.gjest_innmelder_id = g.gjest_innmelder_id
        WHERE i.innmelding_id = @InnmeldingId";

            var result = await connection.QueryAsync<InnmeldingModel, PersonModel, InnmelderModel, SaksbehandlerModel,
                (InnmeldingModel, PersonModel, InnmelderModel, SaksbehandlerModel)>(
                sql,
                (innmelding, person, innmelder, saksbehandler) =>
                {
                    return (innmelding, person, innmelder, saksbehandler);
                },
                new { InnmeldingId = innmeldingId },
                splitOn: "Fornavn,InnmelderId,Stilling");

            return result.FirstOrDefault();
        }


    }
}

