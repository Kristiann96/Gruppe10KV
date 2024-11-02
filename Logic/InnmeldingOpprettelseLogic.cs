using Interface;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



public class InnmeldingOpprettelseLogic : IInnmeldingOpprettelseLogic
{
    private const int NORGE_SRID = 4326;  // WGS 84
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
        GeometriModel geometri,
        string gjesteEpost)
    {
        // Epost validering
        if (!ErGyldigEpost(gjesteEpost))
        {
            throw new ForretningsRegelException("Ugyldig epost-format");
        }

        // GeoJSON validering og konvertering
        if (!ErGyldigGeoJson(geometri.GeometriGeoJson))
        {
            throw new ForretningsRegelException("Ugyldig GeoJSON format");
        }

        var wktMedSrid = KonverterGeoJsonTilWktMedSrid(
            geometri.GeometriGeoJson,
            NORGE_SRID
        );

        // Lagring...
        try
        {
            // TODO: Implementere transaksjonshåndtering
            // Lagre gjesteinnmelder først for å få ID
            var gjesteinnmelder = new GjesteinnmelderModel
            {
                Epost = gjesteEpost
            };

            // Koble gjesteinnmelder til innmelding
            innmelding.GjestInnmelderId = gjesteinnmelder.GjestInnmelderId;

            // Lagre i riktig rekkefølge med transaksjoner
            return true;
        }
        catch (Exception ex)
        {
            // Håndtere feil, rollback etc.
            throw;
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

    private string KonverterGeoJsonTilWktMedSrid(string geoJson, int srid)
    {
        // Her må vi implementere konvertering fra GeoJSON til WKT format
        // og legge til SRID
        // Returnerer format: "ST_GeomFromText('POLYGON(...)', 4326)"

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
            throw new ForretningsRegelException("Kunne ikke konvertere GeoJSON til WKT format");
        }
    }

    private string GeoJsonCoordinaterTilWkt(string type, JsonElement coordinates)
    {
        // Implementere konvertering basert på geometritype
        switch (type)
        {
            case "Point":
            // Implementer Point konvertering
            case "LineString":
            // Implementer LineString konvertering
            case "Polygon":
            // Implementer Polygon konvertering
            default:
                throw new ForretningsRegelException("Ikke støttet geometritype");
        }
    }
}
