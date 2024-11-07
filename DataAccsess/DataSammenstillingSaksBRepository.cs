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
        
        public async Task<(IEnumerable<(InnmeldingModel, PersonModel, Geometri, GjesteinnmelderModel, InnmelderModel)> Data, int TotalPages)> GetOversiktAlleInnmeldingerSaksBAsync(int pageNumber, int pageSize, string searchTerm)
        {
            using var connection = _dbConnection.CreateConnection();

            // Calculate total items for pagination
            var countSql = @"
                SELECT COUNT(*)
                FROM innmelding im
                LEFT JOIN innmelder i ON im.innmelder_id = i.innmelder_id
                LEFT JOIN person p ON i.person_id = p.person_id
                LEFT JOIN geometri g ON im.innmelding_id = g.innmelding_id
                LEFT JOIN gjesteinnmelder gi ON im.gjest_innmelder_id = gi.gjest_innmelder_id
                WHERE im.tittel LIKE @SearchTerm";

            var totalItems = await connection.ExecuteScalarAsync<int>(countSql, new { SearchTerm = "%" + searchTerm + "%" });

            // Calculate total pages
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Main query to fetch paginated data with all necessary fields
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
                    g.innmelding_id,
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
                ORDER BY im.innmelding_id
                LIMIT @PageSize OFFSET @Offset";

            var parameters = new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize,
                SearchTerm = "%" + searchTerm + "%"
            };

            Console.WriteLine($"Executing query with parameters: Offset={parameters.Offset}, PageSize={parameters.PageSize}, SearchTerm={parameters.SearchTerm}");

            var result = await connection.QueryAsync<InnmeldingModel, PersonModel, Geometri, GjesteinnmelderModel, InnmelderModel,
                (InnmeldingModel, PersonModel, Geometri, GjesteinnmelderModel, InnmelderModel)>(
                dataSql,
                (innmelding, person, geometri, gjesteinnmelder, innmelder) =>
                {
                    // Handle Gjesteinnmelder email
                    if (gjesteinnmelder != null)
                    {
                        Console.WriteLine($"GjesteinnmelderEmail before: {gjesteinnmelder.Epost}");
                        gjesteinnmelder.Epost = string.IsNullOrEmpty(gjesteinnmelder.Epost) ? "N/A" : gjesteinnmelder.Epost;
                        Console.WriteLine($"GjesteinnmelderEmail after: {gjesteinnmelder.Epost}");
                    }

                    // Handle Innmelder email
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

            // Log the results for debugging
            var resultList = result.ToList();
            if (resultList.Any())
            {
                foreach (var item in resultList)
                {
                    Console.WriteLine($"InnmeldingId: {item.Item1?.InnmeldingId}");
                    Console.WriteLine($"InnmelderEpost: {item.Item5?.Epost}");
                    Console.WriteLine($"GjestEpost: {item.Item4?.Epost}");
                    Console.WriteLine("---");
                }
            }
            else
            {
                Console.WriteLine("No results found.");
            }

            return (resultList, totalPages);
        }
    }
}
