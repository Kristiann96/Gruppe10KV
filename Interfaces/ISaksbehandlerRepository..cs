using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ISaksbehandlerRepository
    {
        Task<IEnumerable<SaksbehandlerInnmeldingModel>> GetAllAdviserFormsAsync();
        Task<SaksbehandlerInnmeldingModel> GetAdviserFormByIdAsync(int id);
       
    }
}