using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace ServicesInterfaces
{
    public interface IOppdatereInnmeldingService
    {
        Task<OppdatereInnmeldingViewModel> HentInnmeldingForOppdateringAsync(int innmeldingId);
        Task<bool> OppdatereInnmeldingAsync(OppdatereInnmeldingViewModel model);
        Task<bool> OppdatereGeometriAsync(int innmeldingId, string geometriGeoJson);
        Task<bool> SlettInnmeldingAsync(int innmeldingId);
    }
}
