using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("ApiParties/Parties")]
public class PartieControllers : ControllerBase
{
    private readonly PartieContext _context;
    private readonly IConfiguration _configuration;

    public PartieControllers(PartieContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // 🔹 GET : Récupérer toutes les parties (Admin uniquement)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<Partie>>> GetParties()
    {
        var parties = await _context.Parties.ToListAsync();
        return Ok(
            parties.Select(p => new
            {
                p.Id,
                p.Code,
                p.Name,
                p.Chef,
                p.Users,
            })
        );
    }

    // 🔹 GET : Récupérer une partie par son ID (Admin uniquement)
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Partie>> GetPartieById(int id)
    {
        var partie = await _context.Parties.FindAsync(id);
        if (partie == null)
            return NotFound(new { message = "Partie non trouvée." });

        return Ok(
            new
            {
                partie.Id,
                partie.Code,
                partie.Name,
                partie.Chef,
                partie.Users,
            }
        );
    }

    // 🔹 POST : Créer une nouvelle partie (Uniquement pour les utilisateurs authentifiés)
    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult> CreatePartie([FromBody] PartieModel model)
    {
        var username = User.Identity.Name; // 🔥 Récupère le username depuis le JWT
        Console.WriteLine($"Nom utilisateur récupéré depuis le JWT : {username}"); // vérification

        if (string.IsNullOrEmpty(username))
            return Unauthorized(new { message = "Utilisateur non identifié dans le JWT." });

        var allUsers = await _context.Users.ToListAsync(); // 🔥 Charge tous les utilisateurs en mémoire
        foreach (var u in allUsers)
        {
            Console.WriteLine($" - ID: {u.Id}, UserName: [{u.UserName}]");
        }

        var chef = allUsers.FirstOrDefault(u =>
            u.UserName.Trim().ToLower() == username.Trim().ToLower()
        );

        Console.WriteLine(
            $"📌 Résultat de la recherche : {chef?.UserName ?? "Aucun utilisateur trouvé"}"
        );

        Console.WriteLine($"Fyfuu :{chef}");

        if (chef == null)
        {
            Console.WriteLine("⚠️ L'utilisateur n'existe pas dans la base !");
            return Unauthorized(new { message = "Utilisateur non trouvé en base." });
        }

        var newPartie = new Partie
        {
            Name = model.Name,
            Code = model.Code,
            Chef = chef,
            Users = new List<Users> { chef },
        };

        _context.Entry(chef).State = EntityState.Unchanged;

        try
        {
            _context.Parties.Add(newPartie);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPartieById), new { id = newPartie.Id }, newPartie);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur lors de la sauvegarde en base : {ex.Message}");
            return StatusCode(500, new { message = "Erreur interne du serveur." });
        }
    }

    // 🔹 POST : Rejoindre une partie
    [HttpPost("{id:int}/join")]
    [Authorize]
    public async Task<IActionResult> JoinPartie(int id)
    {
        var username = User.Identity.Name;
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

        if (user == null)
            return Unauthorized(new { message = "Utilisateur non trouvé." });

        var partie = await _context
            .Parties.Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (partie == null)
            return NotFound(new { message = "Partie non trouvée." });

        if (partie.Users.Contains(user))
            return BadRequest(new { message = "Vous êtes déjà dans cette partie." });

        partie.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Vous avez rejoint la partie avec succès." });
    }

    // 🔹 DELETE : Supprimer une partie (seulement le chef de la partie)
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

// ✅ Modèle pour la création de partie
public class PartieModel
{
    public string Code { get; set; }
    public string Name { get; set; }
}
