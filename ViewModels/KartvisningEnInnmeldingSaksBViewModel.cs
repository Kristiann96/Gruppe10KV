using Models.Models;
using Models.Entities;
using System;

namespace ViewModels
{
    public class KartvisningEnInnmeldingSaksBViewModel
    {
        public int InnmeldingId { get; set; }
        public string Tittel { get; set; }
        public string Beskrivelse { get; set; }
        public DateTime SisteEndring { get; set; }
        public Status Status { get; set; }
        public Prioritet Prioritet { get; set; }
        public KartType KartType { get; set; }

        // Innmelder informasjon
        public int? InnmelderId { get; set; }
        public string InnmelderType { get; set; }
        public string InnmelderNavn { get; set; }

        // Gjesteinnmelder ID hvis relevant
        public int? GjestInnmelderId { get; set; }

        // Saksbehandler informasjon
        public int? SaksbehandlerId { get; set; }
        public string SaksbehandlerStilling { get; set; }
        public string SaksbehandlerNavn { get; set; }

        // Geometridata
        public Geometri GeometriData { get; set; }
    }
}
