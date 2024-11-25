using Dapper;
using MySqlConnector;
using System.Threading.Tasks;
using Models.Entities;
using Interfaces;
using Models.Models;

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
                im.innmelding_id AS InnmeldingId, 
                im.tittel AS Tittel,
                im.beskrivelse AS Beskrivelse,
                im.status AS Status,
                im.prioritet AS Prioritet,
                im.kart_type AS KartType,
                p.person_id AS PersonId,
                p.fornavn AS Fornavn,
                p.etternavn AS Etternavn,
                p.telefonnummer AS Telefonnummer,
                i.innmelder_id AS InnmelderId,
                i.innmelder_type AS InnmelderType,
                s.saksbehandler_id AS SaksbehandlerId,
                s.stilling AS SaksbehadlderStilling,
                s.jobbepost AS SaksbehandlerJobbepost,
                s.jobbtelefon AS SaksbehandlerJobbtelefon,
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
                    im.innmelding_id AS InnmeldingId,
                    im.tittel AS Tittel,
                    im.beskrivelse AS Beskrivelse,
                    im.siste_endring AS SisteEndring,
                    im.status AS Status,
                    im.prioritet AS Prioritet,
                    p.person_id AS PersonId,
                    p.fornavn AS Fornavn,
                    p.etternavn AS Etternavn,
                    g.geometri_id AS GeometriId,
                    g.innmelding_id AS InnmeldingId,
                    ST_AsText(g.geometri_data) AS GeometriGeoJson,
                    COALESCE(gi.gjest_innmelder_id, 0) AS GjestInnmelderId,
                    COALESCE(gi.epost, '') AS Epost,
                    COALESCE(i.innmelder_id, 0) AS InnmelderId,
                    COALESCE(i.person_id, 0) AS PersonId,
                    COALESCE(i.epost, '') AS Epost
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
            
            var result = await connection.QueryAsync<InnmeldingModel, PersonModel, Geometri, GjesteinnmelderModel, InnmelderModel,
                (InnmeldingModel, PersonModel, Geometri, GjesteinnmelderModel, InnmelderModel)>(
                dataSql,
                (innmelding, person, geometri, gjesteinnmelder, innmelder) =>
                {
                    if (gjesteinnmelder != null)
                    {
                        gjesteinnmelder.Epost = string.IsNullOrEmpty(gjesteinnmelder.Epost) ? "N/A" : gjesteinnmelder.Epost;
                    }

                    if (innmelder != null)
                    {
                        innmelder.Epost = string.IsNullOrEmpty(innmelder.Epost) ? "N/A" : innmelder.Epost;
                    }

                    return (innmelding, person, geometri, gjesteinnmelder, innmelder);
                },
                parameters,
                splitOn: "PersonId,GeometriId,GjestInnmelderId,InnmelderId"
            );
            
            var resultList = result.ToList();

            return (resultList, totalPages);
        }
    }
}

