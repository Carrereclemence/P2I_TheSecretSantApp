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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized(new { message = "Utilisateur non identifié dans le JWT." });

        var chef = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (chef == null)
            return Unauthorized(new { message = "Utilisateur non trouvé en base." });

        var newPartie = new Partie
        {
            Name = model.Name,
            Code = model.Code,
            ChefId = chef.Id,
            Chef = chef,
        };

        _context.Parties.Add(newPartie);
        await _context.SaveChangesAsync();

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
        var username = User.Identity.Name;
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

        if (user == null)
            return Unauthorized(new { message = "Utilisateur non trouvé." });

        var codeRecherche = model.Code?.Trim().ToLower();

        var partie = await _context
            .Parties.Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Code.ToLower() == codeRecherche);

        if (partie == null)
            return NotFound(new { message = "Partie non trouvée." });

        if (partie.Users.Any(u => u.Id == user.Id))
            return BadRequest(new { message = "Vous êtes déjà dans cette partie." });

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
    [HttpPost("{id:int}/tirage")]
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
        if (participants.Count < 2)
            return BadRequest(new { message = "Pas assez de participants." });

        var anciensTirages = _context.Tirages.Where(t => t.PartieId == id);
        _context.Tirages.RemoveRange(anciensTirages);

        var shuffled = participants.OrderBy(p => Guid.NewGuid()).ToList();
        var tirages = new List<Tirage>();

        for (int i = 0; i < shuffled.Count; i++)
        {
            var offr = shuffled[i];
            var dest = shuffled[(i + 1) % shuffled.Count];

            tirages.Add(
                new Tirage
                {
                    PartieId = id,
                    OffrantId = offr.Id,
                    DestinataireId = dest.Id,
                }
            );
        }

        _context.Tirages.AddRange(tirages);
        await _context.SaveChangesAsync();

        return Ok(
            tirages.Select(t => new
            {
                Offrant = participants.First(p => p.Id == t.OffrantId).UserName,
                Destinataire = participants.First(p => p.Id == t.DestinataireId).UserName,
            })
        );
    }

    // GET : Destinataire du mon cadeau
    [HttpGet("{id:int}/mon-destinataire")]
    [Authorize]
    public async Task<IActionResult> GetDestinataire(int id)
    {
        var username = User.Identity.Name;
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
        if (user == null)
            return Unauthorized();

        var destinataire = await _context
            .Tirages.Include(t => t.Destinataire)
            .FirstOrDefaultAsync(t => t.PartieId == id && t.OffrantId == user.Id);

        if (destinataire == null)
            return NotFound(
                new { message = "Aucun destinataire trouvé pour vous dans cette partie." }
            );

        return Ok(
            new
            {
                destinataire.Destinataire.Id,
                destinataire.Destinataire.UserName,
                destinataire.Destinataire.FirstName,
                destinataire.Destinataire.LastName,
            }
        );
    }

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
