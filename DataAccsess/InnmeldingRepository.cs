using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Interface;
using Models.DTOs;

namespace DataAccess
{
    public class InnmeldingRepository : IInnmeldingRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public InnmeldingRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Innmelding> GetInnmeldingByIdAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = "SELECT * FROM innmelding WHERE innmelding_id = @InnmeldingId";
            return await connection.QuerySingleOrDefaultAsync<Innmelding>(sql, new { InnmeldingId = innmeldingId });
        }

        public async Task<InnmeldingDetaljerKartvisningSaksBModel> GetInnmeldingDetaljerByIdAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
        SELECT im.innmelding_id AS InnmeldingId, im.tittel AS Tittel, im.beskrivelse AS Beskrivelse,
               im.siste_endring AS SisteEndring, im.status AS Status, im.prioritet AS Prioritet, im.kart_type AS KartType,
               i.innmelder_id AS InnmelderId, i.innmelder_type AS InnmelderType,
               p.fornavn AS Fornavn, p.etternavn AS Etternavn,
               s.saksbehandler_id AS SaksbehandlerId, s.stilling AS SaksbehandlerStilling,
               g.gjest_innmelder_id AS GjestInnmelderId
        FROM innmelding im
        LEFT JOIN innmelder i ON im.innmelder_id = i.innmelder_id
        LEFT JOIN person p ON i.person_id = p.person_id
        LEFT JOIN saksbehandler s ON im.saksbehandler_id = s.saksbehandler_id
        LEFT JOIN gjesteinnmelder g ON im.gjest_innmelder_id = g.gjest_innmelder_id
        WHERE im.innmelding_id = @InnmeldingId";

            return await connection.QuerySingleOrDefaultAsync<InnmeldingDetaljerKartvisningSaksBModel>(sql, new { InnmeldingId = innmeldingId });
        }


    }

}
