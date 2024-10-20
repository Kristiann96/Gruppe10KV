namespace Models.Entities
{
    //representerer hele entiteten INNMELDING i databasen
    public class InnmeldingEModel
    {
        public int InnmeldID { get; set; }  // Primærnøkkel, auto-increment

        public int InnmelderID { get; set; }  // ID til innmelder, not null

        public DateTime Innmeldingstidspunkt { get; set; } = DateTime.Now;  // Default til nåværende tidspunkt

        public StatusEnum StatusID { get; set; } = StatusEnum.Ny;  // Default til "Ny"

        public int? SaksbehandlerID { get; set; }  // Kan være null inntil en saksbehandler er tildelt

        public DateTime SisteEndring { get; set; } = DateTime.Now;  // Settes automatisk til nåværende tidspunkt

        public string KommuneNR { get; set; } = string.Empty;  // Kommunenummer (påkrevd, default til tom streng)

        public PrioritetsEnum PrioritetID { get; set; } = PrioritetsEnum.IkkeVurdert;  // Default til "Ikke vurdert"

        public string KartID { get; set; } = string.Empty;  // KartID (påkrevd, default til tom streng)
    }

    public enum StatusEnum
    {
        Ny,
        IkkePaBegynt,
        UnderBehandling,
        Pauset,
        IkkeTattTilFølge,
        Avsluttet
    }

    public enum PrioritetsEnum
    {
        IkkeVurdert,
        Lav,
        Moderat,
        Høy,
        Kritisk,
        FarligForLivOgHelse,
        EkstremtHaster
    }
}
