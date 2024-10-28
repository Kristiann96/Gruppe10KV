using System.Threading.Tasks;
using Models.Entities;

namespace Interface
{
    public interface ISaksbehandlerRepository
    {
        Task<Saksbehandler> GetSaksbehandlerByIdAsync(int saksbehandlerId);
    }
}