using System.Collections.Generic;
using Models.Models;
using Models.Entities;
using Newtonsoft.Json;

namespace ViewModels
{
    public class BidraTilKartForbedringViewModel
    {
        public IEnumerable<(Geometri Geometri, InnmeldingModel Innmelding)> GeometriData { get; set; }

        public string GetGeometriDataAsJson()
        {
            if (GeometriData == null) return "[]";

            var formattedData = GeometriData.Select(item => new
            {
                GeometriId = item.Geometri.GeometriId,
                InnmeldingId = item.Geometri.InnmeldingId,
                GeometriGeoJson = item.Geometri.GeometriGeoJson,
                Tittel = item.Innmelding.Tittel
            });

            return JsonConvert.SerializeObject(formattedData);
        }
    }
}