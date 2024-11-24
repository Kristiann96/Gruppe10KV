using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Models.Models;

namespace LogicInterfaces
{
    public interface IInnmeldingLogic
    {
        Task<bool> ValidereOgLagreNyInnmelding(
            InnmeldingModel innmelding, 
            Geometri geometri, 
            string gjesteEpost, 
            bool ErLoggetInn);
        Task<bool> ValidereGeometriDataForOppdatering(int innmeldingId, Geometri geometri);

        Task<bool> ValiderInnmeldingData(InnmeldingModel innmelding);
    }


}
