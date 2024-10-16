using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IInnmelderRepository
    {
        Task<IEnumerable<InnmelderSkjemaModel>> GetAllIncidentsAsync();
        
        Task<InnmelderSkjemaModel> GetIncidentByIdAsync(int id);

        // Returtypen Task<bool> for å indikere om operasjonen var vellykket
        Task<bool> SaveIncidentFormAsync(InnmelderSkjemaModel form);
    }
}
