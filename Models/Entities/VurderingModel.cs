using System;

namespace Models.Entities
{
    public class VurderingModel
    {
        public int VurderingId { get; set; }
        public int InnmeldingId { get; set; }
        public int? InnmelderId { get; set; }  // Nullable siden vi ikke har login ennå
        public string Type { get; set; } = null!;  // enum 'bekreftelse' eller 'avkreftelse'
        public string? Kommentar { get; set; }  // Nullable siden kommentar er valgfri
        public DateTime Dato { get; set; }
    }
}
