using Microsoft.EntityFrameworkCore;

public class PartieContext : DbContext
{
    public DbSet<Partie> Parties { get; set; } = null!;
    public DbSet<Users> Users { get; set; } = null!;
    private readonly string _connectionString;

    public PartieContext(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Partie.db";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(_connectionString);
    }
}
