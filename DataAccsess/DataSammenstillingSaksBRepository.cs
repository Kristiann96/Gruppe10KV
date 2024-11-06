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

            // Calculate total items
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

            // Fetch paginated data
            var dataSql = @"
                SELECT
                    im.innmelding_id AS InnmeldingId,
                    im.tittel AS Tittel,
                    im.beskrivelse AS Beskrivelse,
                    im.siste_endring AS SisteEndring,
                    im.status AS Status,
                    im.prioritet AS Prioritet,
                    im.kart_type AS KartType,
                    p.person_id AS PersonId,
                    p.fornavn AS Fornavn,
                    p.etternavn AS Etternavn,
                    p.telefonnummer AS Telefonnummer,
                    g.geometri_id AS GeometriId,
                    g.innmelding_id AS InnmeldingId,
                    ST_AsText(g.geometri_data) AS GeometriGeoJson,
                    gi.gjest_innmelder_id AS GjestInnmelderId,
                    gi.epost AS Gjestepost,
                    i.innmelder_id AS InnmelderId,
                    i.epost AS InnmelderEpost,
                    i.innmelder_type AS InnmelderType
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
            
            Console.WriteLine($"Executing query: {dataSql}");
            Console.WriteLine($"With parameters: Offset={parameters.Offset}, PageSize={parameters.PageSize}, SearchTerm={parameters.SearchTerm}");

            var result = await connection.QueryAsync<InnmeldingModel, PersonModel, Geometri, GjesteinnmelderModel, InnmelderModel, (InnmeldingModel, PersonModel, Geometri, GjesteinnmelderModel, InnmelderModel)>(
                dataSql,
                (innmelding, person, geometri, gjesteinnmelder, innmelder) => (innmelding, person, geometri, gjesteinnmelder, innmelder),
                parameters,
                splitOn: "PersonId,GeometriId,GjestInnmelderId,InnmelderId"
            );
            
            // Log the result for debugging
            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    var innmelderEpost = item.Item5?.Epost;
                    var gjestEpost = item.Item4?.Epost;

                    Console.WriteLine($"InnmeldingId: {item.Item1.InnmeldingId}, InnmelderEpost: {innmelderEpost}, GjestEpost: {gjestEpost}");

                    if (!string.IsNullOrEmpty(innmelderEpost))
                    {
                        Console.WriteLine($"InnmelderEpost retrieved: {innmelderEpost}");
                    }
                    else
                    {
                        Console.WriteLine("InnmelderEpost is null or empty");
                    }

                    if (!string.IsNullOrEmpty(gjestEpost))
                    {
                        Console.WriteLine($"GjestEpost retrieved: {gjestEpost}");
                    }
                    else
                    {
                        Console.WriteLine("GjestEpost is null or empty");
                    }
                }
            }
            else
            {
                Console.WriteLine("No results found.");
            }
            return (result, totalPages);
        }
    }
}
