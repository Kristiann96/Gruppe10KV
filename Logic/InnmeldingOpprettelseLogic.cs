using Interface;
using Models.Entities;
using System.Text.Json;
using LogicInterfaces;
using Models.Models;
using Models.Exceptions;

namespace Logic
{
    public class InnmeldingOpprettelseLogic : IInnmeldingOpprettelseLogic
    {
        private const double NORGE_MIN_LAT = 57.0;
        private const double NORGE_MAX_LAT = 72.0;
        private const double NORGE_MIN_LON = 4.0;
        private const double NORGE_MAX_LON = 32.0;

        private readonly ITransaksjonsRepository _transaksjonsRepository;

        public InnmeldingOpprettelseLogic(ITransaksjonsRepository transaksjonsRepository)
        {
            _transaksjonsRepository = transaksjonsRepository;
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

            // GeoJSON validering
            try
            {
                using var doc = JsonDocument.Parse(geometri.GeometriGeoJson);
                ValidereGeometriType(doc.RootElement);
                ValidereKoordinater(doc.RootElement);
            }
            catch (JsonException)
            {
                throw new ForretningsRegelExceptionModel("Ugyldig GeoJSON format");
            }
            try
            {
                // Lagre alt i én transaksjon
                return await _transaksjonsRepository.LagreKomplettInnmeldingAsync(
                gjesteEpost,
                innmelding,
                geometri);
            }
            catch (Exception ex)
            {
                throw new ForretningsRegelExceptionModel("Kunne ikke lagre innmeldingen: " + ex.Message);
            }
        }


        private void ValidereGeometriType(JsonElement root)
        {
            if (!root.TryGetProperty("type", out var typeProperty))
            {
                throw new ForretningsRegelExceptionModel("GeoJSON mangler 'type' felt");
            }

            var type = typeProperty.GetString();
            if (type == "Feature")
            {
                if (!root.TryGetProperty("geometry", out var geometry))
                {
                    throw new ForretningsRegelExceptionModel("GeoJSON Feature mangler 'geometry' felt");
                }
                ValidereGeometriType(geometry);
                return;
            }

            var støttedeTyper = new[] { "Point", "LineString", "Polygon", "MultiPoint", "MultiLineString", "MultiPolygon" };
            if (!støttedeTyper.Contains(type))
            {
                throw new ForretningsRegelExceptionModel($"Geometritype '{type}' er ikke støttet");
            }
        }

        private void ValidereKoordinater(JsonElement element)
        {
            JsonElement coordinates;
            if (element.TryGetProperty("type", out var typeProperty) && typeProperty.GetString() == "Feature")
            {
                coordinates = element.GetProperty("geometry").GetProperty("coordinates");
            }
            else
            {
                coordinates = element.GetProperty("coordinates");
            }

            ValidereKoordinaterRekursivt(coordinates);
        }

        private void ValidereKoordinaterRekursivt(JsonElement coordinates)
        {
            if (coordinates.ValueKind == JsonValueKind.Array)
            {
                if (coordinates[0].ValueKind == JsonValueKind.Number)
                {
                    // Dette er et enkelt koordinatpar [lon, lat]
                    var lon = coordinates[0].GetDouble();
                    var lat = coordinates[1].GetDouble();

                    if (lat < NORGE_MIN_LAT || lat > NORGE_MAX_LAT ||
                        lon < NORGE_MIN_LON || lon > NORGE_MAX_LON)
                    {
                        throw new ForretningsRegelExceptionModel(
                            "Koordinater må være innenfor Norge: " +
                            $"Breddegrad {NORGE_MIN_LAT}° til {NORGE_MAX_LAT}°, " +
                            $"Lengdegrad {NORGE_MIN_LON}° til {NORGE_MAX_LON}°");
                    }
                }
                else
                {
                    // Rekursivt sjekk alle underkoordinater
                    foreach (var coord in coordinates.EnumerateArray())
                    {
                        ValidereKoordinaterRekursivt(coord);
                    }
                }
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
    }
}
