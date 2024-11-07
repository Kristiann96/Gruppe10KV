using System.Threading.Tasks;
using Models.Entities;

namespace Interface
{
    public interface ISaksbehandlerRepository
    {
        Task<bool> ErGyldigSaksbehandlerEpost(string epost);
        Task<SaksbehandlerModel?> HentSaksbehandlerMedEpost(string epost);

        //hente saksbehandler data når de er logget inn
        Task<SaksbehandlerModel?> HentSaksbehandler(int personId);
    }
}