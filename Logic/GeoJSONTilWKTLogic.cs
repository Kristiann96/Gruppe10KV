/*using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Models.Exceptions;
using Newtonsoft.Json;

namespace Logic
{
    public class GeoJSONTilWKTLogic
    {
        public static string KonverterGeoJSONTilWKT(string geoJson)
        {
            try
            {
                Console.WriteLine($"Innkommende GeoJSON: {geoJson}"); // Debug logging

                JObject json = JObject.Parse(geoJson);

                // Håndterer både direkte geometri og Feature-objekter
                JObject geometryObject;
                if (json["type"]?.ToString() == "Feature")
                {
                    geometryObject = json["geometry"] as JObject;
                    if (geometryObject == null)
                    {
                        throw new ForretningsRegelExceptionModel("Mangler geometry-objekt i Feature");
                    }
                }
                else
                {
                    geometryObject = json;
                }

                string geometryType = geometryObject["type"]?.ToString();
                var coordinates = geometryObject["coordinates"];

                if (string.IsNullOrEmpty(geometryType) || coordinates == null)
                {
                    throw new ForretningsRegelExceptionModel("Mangler type eller coordinates i geometri");
                }

                string wkt = geometryType switch
                {
                    "Point" => KonverterPunkt(coordinates),
                    "LineString" => KonverterLinje(coordinates),
                    "Polygon" => KonverterPolygon(coordinates),
                    _ => throw new ForretningsRegelExceptionModel($"Geometritype '{geometryType}' er ikke støttet")
                };

                Console.WriteLine($"Konvertert WKT: {wkt}"); // Debug logging
                return wkt;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Feil under konvertering: {ex.Message}"); // Debug logging
                throw new ForretningsRegelExceptionModel($"Feil under konvertering av GeoJSON: {ex.Message}");
            }
        }

        private static string KonverterPunkt(JToken coordinates)
        {
            var x = coordinates[0].ToObject<double>();
            var y = coordinates[1].ToObject<double>();
            return $"POINT({x} {y})";
        }

        private static string KonverterLinje(JToken coordinates)
        {
            var points = coordinates.Select(coord => $"{coord[0]} {coord[1]}");
            return $"LINESTRING({string.Join(", ", points)})";
        }

        private static string KonverterPolygon(JToken coordinates)
        {
            var rings = coordinates.Select(ring =>
                $"({string.Join(", ", ring.Select(coord => $"{coord[0]} {coord[1]}"))})");
            return $"POLYGON({string.Join(", ", rings)})";
        }
    }
}*/