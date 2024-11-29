using System.Threading.Tasks;
using Models.Entities;

namespace Interface
{
    public interface ISaksbehandlerRepository
    {
        
        Task<List<(SaksbehandlerModel, PersonModel)>> HentAlleSaksbehandlereMedPersonAsync();
    }
}