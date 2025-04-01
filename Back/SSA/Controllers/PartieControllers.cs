using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("ApiParties/Parties")]
public class PartieControllers : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public PartieControllers(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // GET : Récupérer toutes les parties (Admin uniquement)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<Partie>>> GetParties()
    {
        var parties = await _context
            .Parties.Include(p => p.Chef)
            .Include(p => p.Users)
            .ToListAsync();

        // Projection manuelle vers un objet anonyme
        return Ok(
            parties.Select(p => new
            {
                p.Id,
                p.Code,
                p.Name,
                Chef = new
                {
                    p.Chef.Id,
                    p.Chef.UserName,
                    p.Chef.FirstName,
                    p.Chef.LastName,
                },
                Users = p
                    .Users.Select(u => new
                    {
                        u.Id,
                        u.UserName,
                        u.FirstName,
                        u.LastName,
                    })
                    .ToList(),
            })
        );
    }

    // GET : Récupérer une partie par son ID
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<Partie>> GetPartieById(int id)
    {
        var partie = await _context
            .Parties.Include(p => p.Chef)
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (partie == null)
            return NotFound(new { message = "Partie non trouvée." });

        return Ok(
            new
            {
                partie.Id,
                partie.Code,
                partie.Name,
                Chef = new
                {
                    partie.Chef.Id,
                    partie.Chef.UserName,
                    partie.Chef.FirstName,
                    partie.Chef.LastName,
                },
                Users = partie.Users.Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FirstName,
                    u.LastName,
                }),
            }
        );
    }

    // GET : Récupérer toutes les parties d'un utilisateur connecté
    [HttpGet("my-parties")]
    [Authorize]
    public async Task<IActionResult> GetMyParties()
    {
        var username = User.Identity.Name;
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
        if (user == null)
            return Unauthorized(new { message = "Utilisateur non trouvé." });

        // Récupère toutes les parties où l'utilisateur figure dans la liste des participants
        var parties = await _context
            .Parties.Include(p => p.Users)
            .Include(p => p.Chef)
            .Where(p => p.Users.Any(u => u.Id == user.Id))
            .ToListAsync();

        if (parties == null || parties.Count == 0)
            return NotFound(new { message = "Vous n'êtes dans aucune partie." });

        var result = parties.Select(p => new
        {
            p.Id,
            p.Code,
            p.Name,
            Chef = p.Chef == null
                ? null
                : new
                {
                    p.Chef.Id,
                    p.Chef.UserName,
                    p.Chef.FirstName,
                    p.Chef.LastName,
                },
            Users = p.Users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.FirstName,
                u.LastName,
            }),
        });
        return Ok(result);
    }

    // POST : Créer une nouvelle partie (authentifié)
    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult> CreatePartie([FromBody] PartieModel model)
    {
        // Récupération de l'ID user stocké dans le token (si vous stockez bien ClaimTypes.NameIdentifier)
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized(new { message = "Utilisateur non identifié dans le JWT." });

        // Récupère l'utilisateur en base
        var chef = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (chef == null)
            return Unauthorized(new { message = "Utilisateur non trouvé en base." });

        // Crée la partie
        var newPartie = new Partie
        {
            Name = model.Name,
            Code = model.Code,
            ChefId = chef.Id, // clé étrangère
            Chef = chef, // navigation
        };

        // Ajoute la nouvelle partie
        _context.Parties.Add(newPartie);
        await _context.SaveChangesAsync();

        // Ajout de Chef dans la liste des participants
        newPartie.Users.Add(chef);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPartieById),
            new { id = newPartie.Id },
            new
            {
                newPartie.Id,
                newPartie.Code,
                newPartie.Name,
                Chef = newPartie.Chef == null
                    ? null
                    : new
                    {
                        newPartie.Chef.Id,
                        newPartie.Chef.UserName,
                        newPartie.Chef.FirstName,
                        newPartie.Chef.LastName,
                    },
                Users = newPartie.Users.Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FirstName,
                    u.LastName,
                }),
            }
        );
    }

    // POST : Rejoindre une partie par code (authentifié)
    [HttpPost("join")]
    [Authorize]
    public async Task<IActionResult> JoinPartieByCode([FromBody] JoinPartieModel model)
    {
        // On récupère le nom d'utilisateur stocké dans ClaimTypes.Name
        var username = User.Identity.Name;
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

        if (user == null)
            return Unauthorized(new { message = "Utilisateur non trouvé." });

        // Normalisation du code : suppression des espaces et conversion en minuscules
        var codeRecherche = model.Code?.Trim().ToLower();

        // Recherche de la partie par son code, en utilisant une comparaison insensible à la casse
        var partie = await _context
            .Parties.Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Code.ToLower() == codeRecherche);

        if (partie == null)
            return NotFound(new { message = "Partie non trouvée." });

        // Vérifie si l’utilisateur est déjà dans la liste
        if (partie.Users.Any(u => u.Id == user.Id))
            return BadRequest(new { message = "Vous êtes déjà dans cette partie." });

        // Ajoute l'utilisateur dans la partie
        partie.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(
            new
            {
                partie.Id,
                partie.Code,
                partie.Name,
                Chef = partie.Chef == null
                    ? null
                    : new
                    {
                        partie.Chef.Id,
                        partie.Chef.UserName,
                        partie.Chef.FirstName,
                        partie.Chef.LastName,
                    },
                Users = partie.Users.Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FirstName,
                    u.LastName,
                }),
            }
        );
    }

    // POST : Tirage au sort (seulement le chef de la partie)
    /*[HttpPost("{id:int}/tirage")]
    [Authorize]
    [ServiceFilter(typeof(PartieChefAuthorizationAttribute))]
    public async Task<IActionResult> TirageSecretSanta(int id)
    {
        var partie = await _context
            .Parties.Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (partie == null)
            return NotFound(new { message = "Partie non trouvée." });

        var participants = partie.Users;

        // Vérifie qu'il y ait au moins 2 participants
        if (participants.Count < 2)
            return BadRequest(new { message = "Pas assez de participants pour le tirage." });

        var shuffled = participants.OrderBy(x => Guid.NewGuid()).ToList();
        var pairs = new Dictionary<int, int>();

        for (int i = 0; i < shuffled.Count; i++)
        {
            var giver = shuffled[i];
            var receiver = shuffled[(i + 1) % shuffled.Count]; // boucle sur elle-même
            pairs[giver.Id] = receiver.Id;
        }

        partie.SecretSantaPairs = pairs;
        await _context.SaveChangesAsync();

        // Retourne les paires créées
        return Ok(
            pairs.Select(pair => new
            {
                Offrant = participants.First(u => u.Id == pair.Key).UserName,
                Destinataire = participants.First(u => u.Id == pair.Value).UserName,
            })
        );
    }*/

    // DELETE : Supprimer une partie (seulement le chef de la partie)
    [HttpDelete("{id:int}")]
    [Authorize]
    [ServiceFilter(typeof(PartieChefAuthorizationAttribute))]
    public async Task<IActionResult> DeletePartie(int id)
    {
        var partie = await _context.Parties.FindAsync(id);
        if (partie == null)
            return NotFound(new { message = "Partie non trouvée." });

        _context.Parties.Remove(partie);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

// Modèle pour POST create
public class PartieModel
{
    public string Code { get; set; }
    public string Name { get; set; }
}

public class JoinPartieModel
{
    public string Code { get; set; }
}
