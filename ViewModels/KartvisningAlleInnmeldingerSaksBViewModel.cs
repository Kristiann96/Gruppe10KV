using System.Collections.Generic;
using Models.Models;

namespace ViewModels
{
    public class KartvisningAlleInnmeldingerSaksBViewModel
    {
        public IEnumerable<Geometri> GeometriData { get; set; }
        public List<Kommune> KommunerData { get; set; }
    }
}

