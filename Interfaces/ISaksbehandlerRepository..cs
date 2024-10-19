using Models;
using Models.Entities;
using Models.SaksbehandlerModels;

namespace Interfaces
{
    public interface ISaksbehandlerRepository
    {
        // Metoder for incident_form tabellen
        Task<IEnumerable<SaksbehandlerIncidentFormModel>> GetAllAdviserFormsAsync();
        Task<SaksbehandlerIncidentFormModel> GetAdviserFormByIdAsync(int id);

        // Metoder for INNMELDING tabellen
        Task<IEnumerable<SaksbehandlerINNMELDINGModel>> GetAllInnmeldingerAsync();
        Task<SaksbehandlerINNMELDINGModel> GetInnmeldingByIdAsync(int innmeldID);
        Task<StatusEnum> GetStatusByInnmeldIdAsync(int innmeldID);
        Task<bool> UpdateStatusAsync(int innmeldID, StatusEnum status);
        Task<bool> UpdateInnmeldingAsync(SaksbehandlerINNMELDINGModel innmelding);
        IEnumerable<StatusEnum> GetAvailableStatuses();
    }
}