using System.Collections.Generic;
using System.Threading.Tasks;

using Models.Entities;

namespace Interface
{
    public interface IInnmeldingRepository
    {
        //HENTING AV DATA

        //OppdatereInnmelding
        Task<IEnumerable<InnmeldingModel>> GetInnmeldingAsync(int innmeldingIdUpdate);

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
        //setter ny tittel og beskrivelse
        Task<bool> OppdatereInnmeldingAsync(InnmeldingModel innmelding);




    }
}