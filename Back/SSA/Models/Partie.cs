public class Partie
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required Users Chef { get; set; }
    public List<Users> Users { get; set; }

    public Partie() { }
}
