using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class InnmeldingDetaljerKartvisningSaksBModel
    {
        public int InnmeldingId { get; set; }
        public string Tittel { get; set; }
        public string Beskrivelse { get; set; }
        public DateTime SisteEndring { get; set; }
        public string Status { get; set; }
        public string Prioritet { get; set; }
        public string KartType { get; set; }

        // Innmelder informasjon
        public int? InnmelderId { get; set; }
        public string InnmelderType { get; set; }
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }

        // Saksbehandler informasjon
        public int? SaksbehandlerId { get; set; }
        public string SaksbehandlerStilling { get; set; }

        // Gjesteinnmelder informasjon
        public int? GjestInnmelderId { get; set; }
    }
}
