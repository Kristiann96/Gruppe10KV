using Models.Models;
using Models.Entities;

namespace ViewModels
{
    public class KartvisningEnEllerFlereInnmeldingSaksBViewModel
    {
        public List<InnmeldingMedDetaljerViewModel> AlleInnmeldinger { get; set; } = new();
    }
    
    public class InnmeldingMedDetaljerViewModel
    {
        public InnmeldingModel Innmelding { get; set; }
        public PersonModel Person { get; set; }
        public InnmelderModel Innmelder { get; set; }
        public SaksbehandlerModel Saksbehandler { get; set; }
        public Geometri Geometri { get; set; }
        public int AntallBekreftelser { get; set; }
        public int AntallAvkreftelser { get; set; }
        public IEnumerable<string> Kommentarer { get; set; }
        
        public string SaksbehandlerNavn => Person != null
            ? $"{Person.Fornavn} {Person.Etternavn}"
            : "Ikke tildelt";
    }
}
