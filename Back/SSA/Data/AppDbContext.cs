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
        // Partie - Chef
        modelBuilder
            .Entity<Partie>()
            .HasOne(p => p.Chef)
            .WithMany()
            .HasForeignKey(p => p.ChefId)
            .OnDelete(DeleteBehavior.Restrict);

        // Partie - Users (many-to-many)
        modelBuilder.Entity<Partie>().HasMany(p => p.Users).WithMany(); // Pas de colonne PartieId dans Users, tout est géré via table de jointure
    }
}
