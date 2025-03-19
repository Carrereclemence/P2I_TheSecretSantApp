using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("ApiParties/Parties")]
public class PartieControllers : ControllerBase
{
    // Un seul DbContext pour Parties et Users
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
            .Parties.Include(p => p.Chef) // pour récupérer l'objet chef
            .Include(p => p.Users) // pour récupérer la liste des participants
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

    // GET : Récupérer une partie par son ID (Admin uniquement)
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
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

        // Facultatif : si vous voulez que le Chef soit dans la liste des participants
        newPartie.Users.Add(chef);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPartieById), new { id = newPartie.Id }, newPartie);
    }

    // POST : Rejoindre une partie (authentifié)
    [HttpPost("{id:int}/join")]
    [Authorize]
    public async Task<IActionResult> JoinPartie(int id)
    {
        // Si vous stockez le pseudo (UserName) dans ClaimTypes.Name
        var username = User.Identity.Name;
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

        if (user == null)
            return Unauthorized(new { message = "Utilisateur non trouvé." });

        var partie = await _context
            .Parties.Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (partie == null)
            return NotFound(new { message = "Partie non trouvée." });

        // Vérifie si l’utilisateur est déjà dans la liste
        if (partie.Users.Any(u => u.Id == user.Id))
            return BadRequest(new { message = "Vous êtes déjà dans cette partie." });

        // Ajoute dans la liste
        partie.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Vous avez rejoint la partie avec succès." });
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
