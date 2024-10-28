using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    class PersonModel
    {
        public class Person
        {
            // Primærnøkkel
            public int PersonId { get; set; }

            // Kolonner med påkrevde verdier
            public string Fornavn { get; set; } = null!;  // NOT NULL betyr at vi kan markere denne som obligatorisk
            public string Etternavn { get; set; } = null!; // NOT NULL
            public string Telefonnummer { get; set; } = null!; // NOT NULL og UNIQUE

           
        }
    }
}
