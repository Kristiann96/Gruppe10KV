using Models.Models;
using Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ViewModels
{
    public class KartvisningEnInnmeldingSaksBViewModel
    {
        // For bakoverkompatibilitet og enkel tilgang til første innmelding
        public InnmeldingModel Innmelding => AlleInnmeldinger?.FirstOrDefault().Item1;
        public PersonModel Person => AlleInnmeldinger?.FirstOrDefault().Item2;
        public InnmelderModel Innmelder => AlleInnmeldinger?.FirstOrDefault().Item3;
        public SaksbehandlerModel Saksbehandler => AlleInnmeldinger?.FirstOrDefault().Item4;
        public Geometri GeometriData => AlleInnmeldinger?.FirstOrDefault().Item5;

        // Ny property for alle innmeldinger
        public List<(InnmeldingModel Innmelding, PersonModel Person, InnmelderModel Innmelder,
            SaksbehandlerModel Saksbehandler, Geometri Geometri)> AlleInnmeldinger
        { get; set; }

        public int AntallBekreftelser { get; set; }
        public int AntallAvkreftelser { get; set; }
        public IEnumerable<string> Kommentarer { get; set; }

        // Helper properties
        public string SaksbehandlerNavn => Person != null
            ? $"{Person.Fornavn} {Person.Etternavn}"
            : "Ikke tildelt";

        public string FormaterSisteEndring => Innmelding?.SisteEndring.ToString("dd.MM.yyyy HH:mm");
    }
}
