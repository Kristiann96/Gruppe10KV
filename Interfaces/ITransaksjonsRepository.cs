using Models.Entities;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface ITransaksjonsRepository
    {
        Task<bool> LagreKomplettInnmeldingAsync(
            string gjesteEpost,
            InnmeldingModel innmelding,
            Geometri geometri);
    }
}
