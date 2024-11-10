
namespace ViewModels;

public class OversiktAlleInnmeldingerSaksBViewModel
{
    public IEnumerable<int> InnmeldingId { get; set; } = Enumerable.Empty<int>();
    public IEnumerable<string> Tittel { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> Status { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> Prioritet { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<DateTime> SisteEndring { get; set; } = Enumerable.Empty<DateTime>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public string SearchTerm { get; set; } = "";
    public IEnumerable<string> InnmelderNavn { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> InnmelderEpost { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> GjestEpost { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> KommuneData { get; set; } = Enumerable.Empty<string>();
}