using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    
    
        public class Geometri
        {
            public int GeometriId { get; set; }
            public int InnmeldingId { get; set; }
            public string GeometriGeoJson { get; set; } = null!;  // Representasjon i GeoJSON-format - lagring som GEOMETRY i databasen VIA REPOSITORY
        }
    
}
