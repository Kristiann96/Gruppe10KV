using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IInnmelderRepository
    {
        Task<IEnumerable<IncidentForm>> GetAllIncidentsAsync();
        Task<IncidentForm> GetIncidentByIdAsync(int id);
        Task SaveIncidentFormAsync(IncidentForm form);
    }
}
