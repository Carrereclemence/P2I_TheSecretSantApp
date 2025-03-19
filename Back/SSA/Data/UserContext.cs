// using Microsoft.EntityFrameworkCore;

// public class UserContext : DbContext
// {
//     public DbSet<Users> Users { get; set; } = null!;
//     public string DbPath { get; private set; }

//     public UserContext(DbContextOptions<UserContext> options)
//         : base(options)
//     {
//         DbPath = "TheSecretSantaApp.db";
//     }

//     // The following configures EF to create a SQLite database file locally
//     protected override void OnConfiguring(DbContextOptionsBuilder options)
//     {
//         // Use SQLite as database
//         options.UseSqlite($"Data Source={DbPath}");
//     }
// }
