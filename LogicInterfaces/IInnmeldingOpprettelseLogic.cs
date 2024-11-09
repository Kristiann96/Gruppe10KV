using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Models.Models;

namespace LogicInterfaces
{
    public interface IInnmeldingOpprettelseLogic
    {
        Task<bool> ValidereOgLagreNyInnmelding(InnmeldingModel innmelding, Geometri geometri, string gjesteEpost);
        Task BareValidereGeometriData(Geometri geometri);
    }
}
