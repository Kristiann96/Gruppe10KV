using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    internal class InnmelderModel
    {
        public class Innmelder
        {
            // Primærnøkkel
            public int InnmelderId { get; set; }

            // Fremmednøkkel som refererer til Person-tabellen
            public int PersonId { get; set; }

            // E-post og Innmelder-type med angitte egenskaper
            public string Epost { get; set; } = null!;  // NOT NULL og unik
            public InnmelderType InnmelderType { get; set; } = InnmelderType.Privat;  // Standardverdi 'privat'

        }

        public enum InnmelderType
        {
            Privat,
            Offentlig,
            Utrykningspersonell,
            Frivillig,
            Firma
        }
    }

   
}
