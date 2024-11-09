using System.Collections.Generic;
using System.Threading.Tasks;

using Models.Entities;

namespace Interface
{
    public interface IInnmeldingRepository
    {
        //HENTING AV DATA

        //Daniel's interface for "OppdatereInnmelding"
        Task<IEnumerable<InnmeldingModel>> GetInnmeldingAsync(int innmeldingIdUpdate);
        Task OppdatereInnmeldingFormAsync(InnmeldingModel innmelding);
        //



        Task<IEnumerable<InnmeldingModel>> GetOversiktAlleInnmeldingerSaksBAsync(int pageNumber, int pageSize, string searchTerm);
        Task<int> GetTotalInnmeldingerTellerSaksBAsync(string searchTerm);

        Task<IEnumerable<InnmeldingModel>> HentInnmeldingerFraInnmelderIdAsync(int innmeldingID);

        //enum
        Task<string> GetStatusEnumValuesAsync();
        Task<string> GetPrioritetEnumValuesAsync();
        Task<string> GetKartTypeEnumValuesAsync();

        //enum fra innmelder - midlertidig bor den også her

        Task<string> GetInnmelderTypeEnumValuesAsync();
        Task<bool> OppdatereEnumSaksBAsync(int innmeldingId, InnmeldingModel model);
        Task<bool> OppdaterSaksbehandler(int innmeldingId, int? saksbehandlerId);
        Task<bool> OppdaterInnmelderType(int innmelderId, InnmeldingModel model);





    }
}