using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    // Déclare la table des utilisateurs
    public DbSet<Users> Users { get; set; } = null!;

    // Déclare la table des parties
    public DbSet<Partie> Parties { get; set; } = null!;

    private readonly string _connectionString;

    // Constructeur : récupère la connexion depuis la configuration
    public AppDbContext(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=TheSecretSantaApp.db";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Exemple de relation “Partie a un Chef (Users)”
        modelBuilder
            .Entity<Partie>()
            .HasOne(p => p.Chef)
            .WithMany() // un même user peut être chef de plusieurs parties
            .HasForeignKey(p => p.ChefId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
