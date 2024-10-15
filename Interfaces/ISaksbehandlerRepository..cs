using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ISaksbehandlerRepository
    {
        Task<IEnumerable<AdviserForm>> GetAllAdviserFormsAsync();
        Task<AdviserForm> GetAdviserFormByIdAsync(int id);
       
    }
}