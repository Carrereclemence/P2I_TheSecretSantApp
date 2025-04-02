//using System.ComponentModel.DataAnnotations.Schema;

using System.ComponentModel.DataAnnotations.Schema;

public class Partie
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public int ChefId { get; set; }
    public Users Chef { get; set; }

    public List<Users> Users { get; set; } = new List<Users>();

    public Partie() { }
}
