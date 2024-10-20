using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;


namespace ViewModels
{
    public class BehandleInnmeldingSaksBViewModel
    {
        public InnmeldingEModel InnmeldingE { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        
    }
}

