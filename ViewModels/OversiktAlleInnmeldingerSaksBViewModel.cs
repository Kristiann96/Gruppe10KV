
namespace ViewModels;

public class OversiktAlleInnmeldingerSaksBViewModel
{
    public IEnumerable<int> InnmeldingId { get; set; }
    public IEnumerable<string> Tittel { get; set; }
    public IEnumerable<string> Status { get; set; }
    public IEnumerable<string> Prioritet { get; set; }
    public IEnumerable<DateTime> SisteEndring { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public string SearchTerm { get; set; }
    public IEnumerable<string> InnmelderNavn { get; set; }
    public IEnumerable<string> InnmelderEpost { get; set; }
    public IEnumerable<string> GjestEpost { get; set; }
    public IEnumerable<string> KommuneData { get; set; }
}