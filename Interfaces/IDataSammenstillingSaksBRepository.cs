using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IDataSammenstillingSaksBRepository
    {

        Task<(InnmeldingModel, PersonModel, InnmelderModel, SaksbehandlerModel)> GetInnmeldingMedDetaljerAsync(
            int innmeldingId);


    }
}