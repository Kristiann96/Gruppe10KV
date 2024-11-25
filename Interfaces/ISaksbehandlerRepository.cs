using System.Threading.Tasks;
using Models.Entities;

namespace Interface
{
    public interface ISaksbehandlerRepository
    {
        Task<bool> ErGyldigSaksbehandlerEpost(string epost);
        Task<SaksbehandlerModel?> HentSaksbehandlerMedEpost(string epost);
        Task<SaksbehandlerModel?> HentSaksbehandler(int personId);
        Task<List<(SaksbehandlerModel, PersonModel)>> HentAlleSaksbehandlereMedPersonAsync();
    }
}