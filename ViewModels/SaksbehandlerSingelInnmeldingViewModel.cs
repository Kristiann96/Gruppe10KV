using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Models.SaksbehandlerModels;

namespace ViewModels
{
    public class SaksbehandlerSingelInnmeldingViewModel
    {
        public SaksbehandlerINNMELDINGModel Innmelding { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        
    }
}

