using Models.Models;
using Models.Entities;

namespace ViewModels
{
    public class KartvisningEnEllerFlereInnmeldingSaksBViewModel
    {
        public List<InnmeldingOversiktViewModel> AlleInnmeldinger { get; set; } = new();
    }

    public class InnmeldingOversiktViewModel
    {
        public int InnmeldingId { get; set; }
        public string Tittel { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Prioritet { get; set; } = string.Empty;
        public string KartType { get; set; } = string.Empty;
        public string InnmelderType { get; set; } = string.Empty;
        public string? SaksbehandlerFornavn { get; set; }
        public string? SaksbehandlerEtternavn { get; set; }

        public GeometriInfo Geometri { get; set; } = new();

        public int AntallBekreftelser { get; set; }
        public int AntallAvkreftelser { get; set; }
        public List<string> Kommentarer { get; set; } = new();

        public string SaksbehandlerNavn =>
            string.IsNullOrEmpty(SaksbehandlerFornavn)
                ? "Ikke tildelt"
                : $"{SaksbehandlerFornavn} {SaksbehandlerEtternavn}";

    }

    public class GeometriInfo
    {
        public string GeoJson { get; set; } = string.Empty;
    }
}
