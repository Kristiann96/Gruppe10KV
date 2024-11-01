using Models.Entities;

namespace ViewModels;

public class OversiktAlleInnmeldingerSaksBViewModel
{
    public IEnumerable<InnmeldingModel> Innmeldinger { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public string SearchTerm { get; set; }
}