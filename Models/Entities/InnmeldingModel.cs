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


            public string Status { get; set; }         // Default 'ny'
            public DateTime SisteEndring { get; set; }             // NOT NULL, oppdateres automatisk
            public string Prioritet { get; set; }   // Default 'ikke_vurdert'

            public string KartType { get; set; }         // Default 'standard'

            public Geometri Geometri { get; set; } = null!;     // NOT NULL
            public string InnmelderType { get; set; }

    }

        // Enums for status, prioritet og kart_type
        public enum Status
        {
            Ny,
            IkkePåBegynt,
            UnderBehandling,
            Pauset,
            Avsluttet,
            IkkeTattTilFølge
        }

        public enum Prioritet
        {
            IkkeVurdert,
            Lav,
            Moderat,
            Høy,
            Kritisk,
            FarligForLivHelse,
            EkstremHaster
        }

        public enum KartType
        {
            Standard,
            Topografisk,
            Sjøkart,
            Ortofoto,
            Administrativt
        }
    
}
