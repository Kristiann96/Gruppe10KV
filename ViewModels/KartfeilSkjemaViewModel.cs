using System.ComponentModel.DataAnnotations;

public class KartfeilSkjemaViewModel
{
    [Required(ErrorMessage = "Tittel må fylles ut")]
    [StringLength(100, ErrorMessage = "Tittel kan ikke være lengre enn 100 tegn")]
    public string Tittel { get; set; }
    
    [Required(ErrorMessage = "Beskrivelse må fylles ut")]
    public string Beskrivelse { get; set; }
    
    [Required]
    public string GeometriGeoJson { get; set; }
    public bool ErNodEtatKritisk { get; set; }
    public string Prioritet { get; set; }
}
