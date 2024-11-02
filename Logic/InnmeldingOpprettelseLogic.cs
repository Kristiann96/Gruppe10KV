using Interface;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LogicInterfaces;
using Models.Models;
using Models.Exceptions;


public class InnmeldingOpprettelseLogic : IInnmeldingOpprettelseLogic
{
    private const int NORGE_SRID = 4326;
    private const double NORGE_MIN_LAT = 57.0;
    private const double NORGE_MAX_LAT = 72.0;
    private const double NORGE_MIN_LON = 4.0;
    private const double NORGE_MAX_LON = 32.0;

    private readonly IGeometriRepository _geometriRepository;
    private readonly IGjesteinnmelderRepository _gjesteinnmelderRepository;
    private readonly IInnmeldingRepository _innmeldingRepository;

    public InnmeldingOpprettelseLogic(
        IGeometriRepository geometriRepository,
        IGjesteinnmelderRepository gjesteinnmelderRepository,
        IInnmeldingRepository innmeldingRepository)
    {
        _geometriRepository = geometriRepository;
        _gjesteinnmelderRepository = gjesteinnmelderRepository;
        _innmeldingRepository = innmeldingRepository;
    }

    public async Task<bool> ValidereOgLagreNyInnmelding(
        InnmeldingModel innmelding,
        Geometri geometri,
        string gjesteEpost)
    {
        // Epost validering
        if (!ErGyldigEpost(gjesteEpost))
        {
            throw new ForretningsRegelExceptionModel("Ugyldig epost-format");
        }

        // GeoJSON validering og konvertering
        if (!ErGyldigGeoJson(geometri.GeometriGeoJson))
        {
            throw new ForretningsRegelExceptionModel("Ugyldig GeoJSON format");
        }

        var wktMedSrid = KonverterGeoJsonTilWktMedSrid(
            geometri.GeometriGeoJson,
            NORGE_SRID
        );

        try
        {
            // 1. Opprett gjesteinnmelder og få ID
            var gjesteinnmelder = new GjesteinnmelderModel { Epost = gjesteEpost };
            var gjesteinnmelderId = await _gjesteinnmelderRepository.OpprettGjesteinnmelderAsync(gjesteinnmelder);

            // 2. Koble gjesteinnmelder-ID til innmeldingen
            innmelding.GjestInnmelderId = gjesteinnmelderId;

            // 3. Lagre innmeldingen og få den genererte innmelding-ID
            var innmeldingId = await _innmeldingRepository.LagreInnmeldingAsync(innmelding);

            // 4. Koble innmelding-ID til geometrien og lagre
            geometri.InnmeldingId = innmeldingId;
            geometri.GeometriGeoJson = wktMedSrid; // Nå konvertert til WKT med SRID
            await _geometriRepository.LagreGeometriAsync(geometri);

            return true;
        }
        catch (Exception ex)
        {
            // Her kunne vi logget feilen
            throw new ForretningsRegelExceptionModel("Kunne ikke lagre innmeldingen: " + ex.Message);
        }
    }

    private bool ErGyldigEpost(string epost)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(epost);
            return addr.Address == epost;
        }
        catch
        {
            return false;
        }
    }

    private bool ErGyldigGeoJson(string geoJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(geoJson);
            return doc.RootElement.TryGetProperty("type", out _) &&
                   doc.RootElement.TryGetProperty("coordinates", out _);
        }
        catch
        {
            return false;
        }
    }

    private string KonverterGeoJsonTilWktMedSrid(string geoJson, int srid)
    {
        try
        {
            var geoJsonObj = JsonDocument.Parse(geoJson);
            var type = geoJsonObj.RootElement.GetProperty("type").GetString();
            var coordinates = geoJsonObj.RootElement.GetProperty("coordinates");

            var wkt = GeoJsonCoordinaterTilWkt(type, coordinates);
            return $"ST_GeomFromText('{wkt}', {srid})";
        }
        catch
        {
            throw new ForretningsRegelExceptionModel("Kunne ikke konvertere GeoJSON til WKT format");
        }
    }

    private string GeoJsonCoordinaterTilWkt(string type, JsonElement coordinates)
    {
        return type switch
        {
            "Point" => KonverterPoint(coordinates),
            "LineString" => KonverterLineString(coordinates),
            "Polygon" => KonverterPolygon(coordinates),
            _ => throw new ForretningsRegelExceptionModel("Ikke støttet geometritype")
        };
    }

    private string KonverterPoint(JsonElement coordinates)
    {
        return $"POINT({coordinates[0]} {coordinates[1]})";
    }

    private string KonverterLineString(JsonElement coordinates)
    {
        var points = coordinates.EnumerateArray()
            .Select(p => $"{p[0]} {p[1]}")
            .ToArray();
        return $"LINESTRING({string.Join(", ", points)})";
    }

    private string KonverterPolygon(JsonElement coordinates)
    {
        var rings = coordinates.EnumerateArray()
            .Select(ring =>
            {
                var points = ring.EnumerateArray()
                    .Select(p => $"{p[0]} {p[1]}")
                    .ToArray();
                return $"({string.Join(", ", points)})";
            })
            .ToArray();
        return $"POLYGON({string.Join(", ", rings)})";
    }
}
