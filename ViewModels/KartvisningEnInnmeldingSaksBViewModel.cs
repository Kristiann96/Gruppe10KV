using Models.Models;
using Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ViewModels
{
    public class KartvisningEnInnmeldingSaksBViewModel
    {
        // Innmelding med alle nødvendige detaljer
        public InnmeldingModel Innmelding { get; set; }

        // Person-relatert info (for saksbehandler navn)
        public PersonModel Person { get; set; }

        // Innmelder info for type
        public InnmelderModel Innmelder { get; set; }

        // Saksbehandler info
        public SaksbehandlerModel Saksbehandler { get; set; }

        // Geometridata 
        public Geometri GeometriData { get; set; }

        // Helper properties for å forenkle bruken i view
        public string SaksbehandlerNavn => Person != null
            ? $"{Person.Fornavn} {Person.Etternavn}"
            : "Ikke tildelt";

        public string FormaterSisteEndring => Innmelding.SisteEndring.ToString("dd.MM.yyyy HH:mm");
    }
}
