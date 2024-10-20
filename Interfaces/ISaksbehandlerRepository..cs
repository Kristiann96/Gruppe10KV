
using Models.Entities;


namespace Interfaces
{
    public interface ISaksbehandlerRepository
    {
        // Metoder for incident_form tabellen
        Task<IEnumerable<SaksbehandlerIncidentFormModel>> GetAllAdviserFormsAsync();
        Task<SaksbehandlerIncidentFormModel> GetAdviserFormByIdAsync(int id);

        // Metoder for INNMELDING tabellen
        Task<IEnumerable<InnmeldingEModel>> GetAllInnmeldingerAsync();
        Task<InnmeldingEModel> GetInnmeldingByIdAsync(int innmeldID);
        Task<StatusEnum> GetStatusByInnmeldIdAsync(int innmeldID);
        Task<bool> UpdateStatusAsync(int innmeldID, StatusEnum status);
        Task<bool> UpdateInnmeldingAsync(InnmeldingEModel innmeldingE);
        IEnumerable<StatusEnum> GetAvailableStatuses();
    }
}