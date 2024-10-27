﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    

        public class Innmelding
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
            public Status Status { get; set; } = Status.Ny;        // Default 'ny'
            public DateTime SisteEndring { get; set; }             // NOT NULL, oppdateres automatisk
            public Prioritet Prioritet { get; set; } = Prioritet.IkkeVurdert;  // Default 'ikke_vurdert'
            public KartType KartType { get; set; } = KartType.Standard;        // Default 'standard'
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
