using Microsoft.EntityFrameworkCore;

// Classe représentant le contexte de base de données de l'application
public class AppDbContext : DbContext
{
    // Représente la table des utilisateurs
    public DbSet<Users> Users { get; set; } = null!;

    // Représente la table des parties (groupes Secret Santa)
    public DbSet<Partie> Parties { get; set; } = null!;

    // Représente la table des tirages Secret Santa
    public DbSet<Tirage> Tirages { get; set; } = null!;

    private readonly string _connectionString;

    // Constructeur : récupère la chaîne de connexion depuis la configuration
    public AppDbContext(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=TheSecretSantaApp.db"; // Fallback vers un fichier SQLite local
    }

    // Configure le fournisseur de base de données (ici SQLite)
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(_connectionString);
    }

    // Configure les relations entre les entités
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relation 1 partie => 1 chef (un utilisateur)
        // Un chef peut gérer plusieurs parties, mais ne peut pas être supprimé si une partie dépend de lui
        modelBuilder
            .Entity<Partie>()
            .HasOne(p => p.Chef)
            .WithMany()
            .HasForeignKey(p => p.ChefId)
            .OnDelete(DeleteBehavior.Restrict); // Empêche la suppression du chef s’il a des parties

        // Relation many-to-many entre Partie et Users
        // Un utilisateur peut appartenir à plusieurs parties et inversement
        modelBuilder.Entity<Partie>().HasMany(p => p.Users).WithMany();

        // Relation 1 tirage => 1 partie
        // Lorsqu'une partie est supprimée, tous les tirages associés sont aussi supprimés
        modelBuilder
            .Entity<Tirage>()
            .HasOne(t => t.Partie)
            .WithMany()
            .HasForeignKey(t => t.PartieId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete : supprime les tirages liés à la partie
    }
}
