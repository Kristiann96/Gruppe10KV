using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class DropDownDummySaksBViewModel
    {
        public InnmeldingEModel InnmeldingE { get; set; }
        public List<SelectListItem> StatusList { get; set; }

    }
}
