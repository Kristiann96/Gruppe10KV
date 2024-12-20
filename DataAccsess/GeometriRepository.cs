﻿using Dapper;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interface;
using Models.Models;
using Interfaces;
using Models.Entities;


namespace DataAccess
{
    public class GeometriRepository : IGeometriRepository
    {
        private readonly DapperDBConnection _dbConnection;

        public GeometriRepository(DapperDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Geometri>> GetAllGeometriAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            var sql =
                "SELECT geometri_id AS GeometriId, innmelding_id AS InnmeldingId, ST_AsGeoJSON(geometri_data) AS GeometriGeoJson FROM geometri;";
            return await connection.QueryAsync<Geometri>(sql);
        }

        public async Task<Geometri> GetGeometriByInnmeldingIdAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql =
                "SELECT geometri_id, innmelding_id, ST_AsGeoJSON(geometri_data) AS GeometriGeoJson FROM geometri WHERE innmelding_id = @InnmeldingId";
            return await connection.QuerySingleOrDefaultAsync<Geometri>(sql, new { InnmeldingId = innmeldingId });
        }

        public async Task<IEnumerable<(Geometri Geometri, InnmeldingModel Innmelding)>> GetAktiveGeometriMedInnmeldingAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
        SELECT 
            g.geometri_id AS GeometriId, 
            g.innmelding_id AS InnmeldingId, 
            ST_AsGeoJSON(g.geometri_data) AS GeometriGeoJson,
            i.innmelding_id,
            i.tittel AS Tittel,
            i.status AS Status
        FROM geometri g
        INNER JOIN innmelding i ON g.innmelding_id = i.innmelding_id
        WHERE i.status NOT IN ('pauset', 'avsluttet', 'ikke_tatt_til_følge')";

            var result = await connection.QueryAsync<Geometri, InnmeldingModel, (Geometri, InnmeldingModel)>(
                sql,
                (geometri, innmelding) => (geometri, innmelding),
                splitOn: "innmelding_id"
            );

            return result;
        }

        public async Task<bool> OppdatereGeometriAsync(int innmeldingId, string geometriGeoJson)
        {
            using var connection = _dbConnection.CreateConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var sql = @"
            UPDATE geometri 
            SET geometri_data = ST_GeomFromGeoJSON(@GeometriGeoJson)
            WHERE innmelding_id = @InnmeldingId";

                var parameters = new
                {
                    InnmeldingId = innmeldingId,
                    GeometriGeoJson = geometriGeoJson
                };

                var rowsAffected = await connection.ExecuteAsync(sql, parameters, transaction);

                if (rowsAffected == 0)
                {
                    throw new KeyNotFoundException($"Ingen geometri funnet for innmelding_id {innmeldingId}");
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Geometri> GetForBehandleInnmedingAsync(int innmeldingId)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql =
                "SELECT ST_AsGeoJSON(geometri_data) AS GeometriGeoJson FROM geometri WHERE innmelding_id = @InnmeldingId";
            return await connection.QuerySingleOrDefaultAsync<Geometri>(sql, new { InnmeldingId = innmeldingId });
        }

        
    }

}











