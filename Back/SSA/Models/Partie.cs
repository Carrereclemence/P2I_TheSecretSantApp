//using System.ComponentModel.DataAnnotations.Schema;

public class Partie
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public int ChefId { get; set; } // 🔥 Ajouter l'ID du chef
    public Users Chef { get; set; } // 🔗 Relation avec Users

    public List<Users> Users { get; set; } = new List<Users>();
    //[NotMapped]
    //public Dictionary<int, int> SecretSantaPairs { get; set; } = new Dictionary<int, int>();

    public Partie() { }
}
