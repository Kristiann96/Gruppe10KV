
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.SaksbehandlerModels;




namespace ViewModels
{
    public class SaksbehandlerSingelInnmeldingViewModel
    {
        public int InnmeldID { get; set; }  // ID til saken
        public StatusEnum CurrentStatus { get; set; }  // Nåværende status
        public List<SelectListItem> StatusList { get; set; }  // Dropdown for alle statuser
    }
}
