using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{


    public class InnmelderModel
    {
        // Primærnøkkel
        public int InnmelderId { get; set; }

        // Fremmednøkkel som refererer til Person-tabellen
        public int PersonId { get; set; }

        // E-post og Innmelder-type med angitte egenskaper
        public string Epost { get; set; } = null!;  // NOT NULL og unik
        public string InnmelderType { get; set; }   // enum Standardverdi 'privat' de andre verdiene er: 'offentlig', 'utrykningspersonell', 'frivillig', 'firma'

    }


}
