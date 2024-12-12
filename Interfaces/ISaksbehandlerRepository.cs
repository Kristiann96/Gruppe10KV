using System.Threading.Tasks;
using Models.Entities;

namespace Interface
{
    public interface ISaksbehandlerRepository
    {

        Task<List<(SaksbehandlerModel, PersonModel)>> HentAlleSaksbehandlereMedPersonAsync();
        Task<(SaksbehandlerModel? Saksbehandler, PersonModel? Person)> HentSaksbehandlerNavnAsync(int innmeldingId);
    }
}