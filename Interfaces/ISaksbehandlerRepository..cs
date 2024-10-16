using Models;

namespace Interfaces;

public interface ISaksbehandlerRepository
{
    Task<IEnumerable<SaksbehandlerInnmeldingModel>> GetAllAdviserFormsAsync();
    Task<SaksbehandlerInnmeldingModel> GetAdviserFormByIdAsync(int id);
}