using System.Collections.Generic;using System.Runtime.InteropServices;
using Models.Models;
using Newtonsoft.Json;

namespace ViewModels
{
    public class KartvisningAlleInnmeldingerSaksBViewModel
    {
        public IEnumerable<Geometri> GeometriData { get; set; }
        public List<Kommune> KommunerData { get; set; }

        // Metoder for å formatere data for view
        public string GetGeometriDataAsJson()
        {
            if (GeometriData == null) return "[]";
            return JsonConvert.SerializeObject(GeometriData);
        }

        public string GetKommunerDataAsJson()
        {
            if (KommunerData == null) return "[]";
            return JsonConvert.SerializeObject(KommunerData);
        }
    }
}  
