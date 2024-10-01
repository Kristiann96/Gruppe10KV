namespace Models;

public class IncidentFormModel
{
    public required string Subject { get; set; }
    public bool Uttrykning { get; set; }
    public bool Something { get; set; }
    public bool AttachFile { get; set; }
    public string? Description { get; set; }
}