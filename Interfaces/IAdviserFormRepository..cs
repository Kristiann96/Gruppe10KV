using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IAdviserFormRepository
    {
        Task<IEnumerable<AdviserForm>> GetAllAdviserFormsAsync();
        Task<AdviserForm> GetAdviserFormByIdAsync(int id);
       
    }
}