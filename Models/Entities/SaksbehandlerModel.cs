using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{


    public class SaksbehandlerModel
    {
        // Primærnøkkel
        public int SaksbehandlerId { get; set; }

        // Fremmednøkkel som refererer til Person-tabellen
        public int PersonId { get; set; }

        // Egenskaper for jobbepost, jobbtelefon og stilling
        public string Jobbepost { get; set; } = null!;    // NOT NULL og unik
        public string Jobbtelefon { get; set; } = null!;  // NOT NULL og unik
        public string Stilling { get; set; }   // Standardverdi: 'rådgiver' de andre er: leder, tekniker, administrator, prosjektleder, superbruker
                                              
    }

}
