using Models.Entities;

namespace Interface
{


    public interface IIncidentFormRepository


    {
        Task<IEnumerable<IncidentFormModel>> GetAllAdviserFormsAsync();
        Task<IncidentFormModel> GetAdviserFormByIdAsync(int id);

        Task<IEnumerable<IncidentFormModel>> GetAllIncidentsAsync();

        Task<IncidentFormModel> GetIncidentByIdAsync(int id);

        // Returtypen Task<bool> for å indikere om operasjonen var vellykket
        Task<bool> SaveIncidentFormAsync(IncidentFormModel form);
    }
}