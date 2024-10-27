using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Kommune
    {
        public string Navn { get; set; }
        public string Kommunenummer { get; set; }
        public string Geometri { get; set; } // Anta at geometri er en GeoJSON-streng, juster om nødvendig
    }
}

