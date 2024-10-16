using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IInnmelderRepository
    {
        Task<IEnumerable<InnmelderSkjemaModel>> GetAllIncidentsAsync();
        Task<InnmelderSkjemaModel> GetIncidentByIdAsync(int id);
        Task SaveIncidentFormAsync(InnmelderSkjemaModel form);
    }
}
