using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Entities;

namespace Interface
{
    public interface IVurderingRepository
    {
        Task<(int antallBekreftelser, int antallAvkreftelser)> HentAntallVurderingerAsync(int innmeldingId);
        Task<IEnumerable<VurderingModel>> HentVurderingerForInnmeldingAsync(int innmeldingId);
        Task<int> LeggTilVurderingAsync(VurderingModel vurdering);
        Task<IEnumerable<string>> HentKommentarerForInnmeldingAsync(int innmeldingId);
    }
}
