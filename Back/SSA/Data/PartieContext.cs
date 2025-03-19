// using Microsoft.EntityFrameworkCore;

// public class PartieContext : DbContext
// {
//     public DbSet<Partie> Parties { get; set; } = null!;
//     //public DbSet<Users> Users { get; set; } = null!;
//     private readonly string _connectionString;

//     public PartieContext(IConfiguration configuration)
//     {
//         _connectionString =
//             configuration.GetConnectionString("DefaultConnection")
//             ?? "Data Source=TheSecretSantaApp.db"; // 🔥 Changer ici aussi
//     }

//     protected override void OnConfiguring(DbContextOptionsBuilder options)
//     {
//         options.UseSqlite(_connectionString);
//     }
//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         // modelBuilder
//         //     .Entity<Partie>()
//         //     .HasOne(p => p.Chef)
//         //     .WithMany() // 🔥 Un chef peut avoir plusieurs parties
//         //     .HasForeignKey(p => p.ChefId)
//         //     .OnDelete(DeleteBehavior.Restrict); // 🔥 Empêche la suppression en cascade

//         // modelBuilder
//         //     .Entity<Partie>()
//         //     .HasMany(p => p.Users)
//         //     .WithMany() // 🔥 Évite de créer une dépendance circulaire
//         //     .UsingEntity(j => j.ToTable("PartieUsers")); // 🔥 Table de jointure explicite
//         modelBuilder.Entity<Partie>().Ignore(p => p.Chef);
//         modelBuilder.Entity<Partie>().Ignore(p => p.Users);
//     }
// }
