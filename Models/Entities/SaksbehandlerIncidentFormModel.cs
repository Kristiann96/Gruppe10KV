namespace Models.Entities;

public class SaksbehandlerIncidentFormModel
{
    public int Id { get; set; }
    public required string Subject { get; set; }
    public bool Uttrykning { get; set; }
    public bool Something { get; set; }
    public bool AttachFile { get; set; }
    public required string Description { get; set; }
    public required string GeoJson { get; set; }
}