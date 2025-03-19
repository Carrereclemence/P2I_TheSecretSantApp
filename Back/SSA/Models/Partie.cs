public class Partie
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public int ChefId { get; set; } // 🔥 Ajouter l'ID du chef
    public Users Chef { get; set; } // 🔗 Relation avec Users

    public List<Users> Users { get; set; } = new List<Users>();
    public Partie() { }
}
