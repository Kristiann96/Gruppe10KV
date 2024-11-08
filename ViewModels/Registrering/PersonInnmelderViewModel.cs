using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Registrering
{
    public class PersonInnmelderViewModel
    {
        public string Fornavn { get; set; } = null!;
        public string Etternavn { get; set; } = null!;
        public string Telefonnummer { get; set; } = null!;
        public string Epost { get; set; } = null!;
    }
}
