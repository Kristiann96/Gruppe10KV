using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    
    
        public class Tilbakemelding
        {
            // Primærnøkkel
            public int TilbakemeldingsId { get; set; }

            // Fremmednøkler
            public int InnmeldingId { get; set; }
            public int SaksbehandlerId { get; set; }

            //andre felt
            public DateTime Dato { get; set; } = DateTime.Now;  // Setter nåværende tidspunkt som standard
            public string Innhold { get; set; } = null!;        // NOT NULL, representerer tekstinnholdet i tilbakemeldingen
        }


    
}
