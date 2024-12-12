using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace ServicesInterfaces
{
    public interface IKartvisningEnEllerFlereInnmeldingSaksBService
    {
        Task<KartvisningEnEllerFlereInnmeldingSaksBViewModel> HentKartvisningForEnkeltInnmeldingAsync(int innmeldingId);
        Task<KartvisningEnEllerFlereInnmeldingSaksBViewModel> HentKartvisningForFlereInnmeldingerAsync(IEnumerable<int> innmeldingId);
    }
}


