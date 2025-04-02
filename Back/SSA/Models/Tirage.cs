public class Tirage
{
    public int Id { get; set; }

    public int PartieId { get; set; }
    public Partie Partie { get; set; }

    public int OffrantId { get; set; }
    public Users Offrant { get; set; }

    public int DestinataireId { get; set; }
    public Users Destinataire { get; set; }
    public Tirage() { }
}
