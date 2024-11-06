using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Entities;

namespace Interface
{
    public interface IVurderingRepository
    {
        // Henter antall bekreftelser og avkreftelser for en innmelding
        Task<(int antallBekreftelser, int antallAvkreftelser)> HentAntallVurderingerAsync(int innmeldingId);

        // Henter alle vurderinger for en innmelding
        Task<IEnumerable<VurderingModel>> HentVurderingerForInnmeldingAsync(int innmeldingId);

        // Legger til en ny vurdering
        Task<int> LeggTilVurderingAsync(VurderingModel vurdering);

        // Henter alle kommentarer for en innmelding
        Task<IEnumerable<string>> HentKommentarerForInnmeldingAsync(int innmeldingId);
    }
}
