using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    

        public class InnmeldingModel
        {
            // Primærnøkkel
            public int InnmeldingId { get; set; }

            // Fremmednøkler, valgfritt med nullable typer (default NULL)
            public int? InnmelderId { get; set; }
            public int? SaksbehandlerId { get; set; }
            public int? GjestInnmelderId { get; set; }

            // Andre egenskaper
            public string Tittel { get; set; } = null!;            // NOT NULL
            public string Beskrivelse { get; set; } = null!;       // NOT NULL
            public DateTime Innmeldingstidspunkt { get; set; }     // NOT NULL, default til nåværende timestamp

            public string Status { get; set; }         // enum Default 'ny' de andre verdiene er: 'ikke_påbegynt', 'under_behandling', 'pauset', 'avsluttet', 'ikke_tatt_til_følge'
            public DateTime SisteEndring { get; set; }             // NOT NULL, oppdateres automatisk
            public string Prioritet { get; set; }   // enum Default 'ikke_vurdert' de andre verdiene er: 'lav', 'moderat', 'høy', 'kritisk', 'farlig_for_liv_helse', 'ekstrem_haster'

            public string KartType { get; set; }         // enum Default 'standard' de andre verdiene er: 'topografisk', 'sjøkart', 'ortofoto', 'administrativt'

       
            public string InnmelderType { get; set; }

    
        }
       
    
}
