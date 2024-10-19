using Models;
using Models.Entities;

namespace Interfaces;

public interface IInnmelderRepository
{
    Task<IEnumerable<InnmelderIncidentFormModel>> GetAllIncidentsAsync();

    Task<InnmelderIncidentFormModel> GetIncidentByIdAsync(int id);

    // Returtypen Task<bool> for å indikere om operasjonen var vellykket
    Task<bool> SaveIncidentFormAsync(InnmelderIncidentFormModel form);
}