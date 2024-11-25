using System.Collections.Generic;
using Models.Entities;

namespace ViewModels
{
    public class MineInnmeldingerViewModel
    {
        public IEnumerable<InnmelderModel> InnmelderId { get; set; }
        public IEnumerable<InnmeldingModel> Innmeldinger { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string CurrentSortColumn { get; set; } = "SisteEndring";
        public string CurrentSortOrder { get; set; }
    }
}
