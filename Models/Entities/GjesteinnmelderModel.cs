using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    
    
        public class GjesteinnmelderModel
        {
            // Primærnøkkel
            public int GjestInnmelderId { get; set; }

            // Epost for gjesteinnmelder, unik og NOT NULL
            public string Epost { get; set; } = null!;  // NOT NULL og unik
        }

    
}
