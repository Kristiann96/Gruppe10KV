using Microsoft.AspNetCore.Mvc.Rendering;
using Models.SaksbehandlerModels;

namespace ViewModels
{
    public class SaksbehandlerSingelInnmeldingViewModel
    {
        public SaksbehandlerINNMELDINGModel Innmelding { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        public StatusEnum CurrentStatus { get; set; }  // Dette er det feltet som holder den nåværende statusen

    }
}

