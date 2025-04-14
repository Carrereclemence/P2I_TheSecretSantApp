using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new AppDbContext(serviceProvider.GetRequiredService<IConfiguration>()))
        {
            context.Database.EnsureCreated();

            // S'il y a déjà des utilisateurs, on n'en crée pas de nouveaux
            if (context.Users.Any())
                return;

            // 1. Utilisateurs
            var admin = new Users
            {
                UserName = "admin",
                FirstName = "Admin",
                LastName = "Admin",
                Password = "admin",
                Admin = true,
            };

            var clems = new Users
            {
                UserName = "clems",
                FirstName = "Clemence",
                LastName = "Carrere",
                Password = "clems",
                Admin = false,
            };

            var fanfan = new Users
            {
                UserName = "fanfan",
                FirstName = "Francois",
                LastName = "Robillard",
                Password = "fanfan",
                Admin = false,
            };

            var ibou = new Users
            {
                UserName = "ibou",
                FirstName = "Ibrahima",
                LastName = "Niang",
                Password = "ibou",
                Admin = false,
            };

            context.Users.AddRange(admin, clems, fanfan, ibou);
            context.SaveChanges();

            // 2. Partie
            var partie = new Partie
            {
                Name = "Noël 2025",
                Code = "XMAS25",
                ChefId = clems.Id,
                Chef = clems,
            };

            partie.Users.Add(admin);
            partie.Users.Add(clems);
            partie.Users.Add(fanfan);
            partie.Users.Add(ibou);

            context.Parties.Add(partie);
            context.SaveChanges();

            // 3. Tirage
            var shuffled = partie.Users.OrderBy(_ => Guid.NewGuid()).ToList();
            for (int i = 0; i < shuffled.Count; i++)
            {
                var offr = shuffled[i];
                var dest = shuffled[(i + 1) % shuffled.Count];

                context.Tirages.Add(
                    new Tirage
                    {
                        PartieId = partie.Id,
                        OffrantId = offr.Id,
                        DestinataireId = dest.Id,
                    }
                );
            }

            context.SaveChanges();
        }
    }
}
