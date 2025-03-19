using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("ApiUsers/Users")]
public class UsersControllers : ControllerBase
{
    // Remplacez l'ancien UserContext par AppDbContext
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public UsersControllers(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // 🔍 GET: Récupérer son propre profil (Utilisateur connecté)
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> GetCurrentUser()
    {
        // On récupère le nom d'utilisateur stocké dans ClaimTypes.Name
        var username = User.Identity.Name;
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

        if (user == null)
            return NotFound(new { message = "Utilisateur non trouvé." });

        return Ok(
            new
            {
                user.Id,
                user.UserName,
                user.FirstName,
                user.LastName,
                user.Admin,
            }
        );
    }

    // 🔐 GET: Récupérer tous les utilisateurs (Admin uniquement)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(
            users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.FirstName,
                u.LastName,
                u.Admin,
            })
        );
    }

    // 🔐 GET: Récupérer un utilisateur par son ID (Admin uniquement)
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Users>> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "Utilisateur non trouvé." });

        return Ok(
            new
            {
                user.Id,
                user.UserName,
                user.FirstName,
                user.LastName,
                user.Admin,
            }
        );
    }

    // ➕ POST: Créer un nouvel utilisateur (Inscription)
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register([FromBody] RegisterModel model)
    {
        // Vérifie si le nom d'utilisateur existe déjà
        if (_context.Users.Any(u => u.UserName == model.UserName))
            return BadRequest(new { message = "Nom d'utilisateur déjà pris." });

        var newUser = new Users
        {
            UserName = model.UserName,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Password = model.Password, // (en clair, non sécurisé, juste un exemple)
            Admin = model.Admin,
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
    }

    // 🔑 POST: Connexion utilisateur
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        // Recherche l'utilisateur
        var user = await _context.Users.SingleOrDefaultAsync(u =>
            u.UserName == model.UserName && u.Password == model.Password
        );

        if (user == null)
        {
            return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect." });
        }

        // Génère le token JWT
        var tokenString = GenerateJWTToken(user);
        return Ok(new { Token = tokenString, Message = "Connexion réussie !" });
    }

    // 🔄 PUT: Modifier un utilisateur (Admin uniquement)
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserModel model)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "Utilisateur non trouvé." });

        // Mise à jour uniquement des champs envoyés
        user.FirstName = model.FirstName ?? user.FirstName;
        user.LastName = model.LastName ?? user.LastName;
        user.Password = model.Password ?? user.Password;
        user.Admin = model.Admin;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 🗑 DELETE: Supprimer un utilisateur (Admin uniquement)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "Utilisateur non trouvé." });

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 🔐 Génération du token JWT
    private string GenerateJWTToken(Users user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
        );
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            // Enregistre l'Id de l'utilisateur dans NameIdentifier
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            // Enregistre le UserName dans Name
            new Claim(ClaimTypes.Name, user.UserName),
            // Rôle : Admin ou User
            new Claim(ClaimTypes.Role, user.Admin ? "Admin" : "User"),
        };

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"], // Issuer
            _configuration["Jwt:Issuer"], // Audience
            claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// Modèles pour la validation des requêtes
public class LoginModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class RegisterModel
{
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public bool Admin { get; set; }
}

public class UpdateUserModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public bool Admin { get; set; }
}
