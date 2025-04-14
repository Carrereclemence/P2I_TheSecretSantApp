using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

// Contrôleur API pour la gestion des utilisateurs
[ApiController]
[Route("ApiUsers/Users")]
public class UsersControllers : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    // Injection du contexte BDD et configuration (pour les clés JWT)
    public UsersControllers(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // Retourne le profil de l'utilisateur actuellement connecté (via JWT)
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> GetCurrentUser()
    {
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

    // Récupère tous les utilisateurs (accessible uniquement aux admins)
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

    // Récupère un utilisateur par son ID (admin uniquement)
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

    // Inscription d'un nouvel utilisateur (anonyme autorisé)
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
            Password = model.Password, // ⚠️ Mot de passe non hashé
            Admin = model.Admin,
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
    }

    // Connexion utilisateur avec génération d'un token JWT si les identifiants sont corrects
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u =>
            u.UserName == model.UserName && u.Password == model.Password // ⚠️ Pas de hash
        );

        if (user == null)
            return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect." });

        var tokenString = GenerateJWTToken(user);
        return Ok(new { Token = tokenString, Message = "Connexion réussie !" });
    }

    // Mise à jour des infos d’un utilisateur (nécessite d’être connecté)
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserModel model)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "Utilisateur non trouvé." });

        // Mise à jour conditionnelle des champs
        user.FirstName = model.FirstName ?? user.FirstName;
        user.LastName = model.LastName ?? user.LastName;
        user.Password = string.IsNullOrWhiteSpace(model.Password) ? user.Password : model.Password;

        if (model.Admin.HasValue)
            user.Admin = model.Admin.Value;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Suppression d'un utilisateur (admin uniquement)
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

    // Génère un token JWT contenant l’ID, le nom et le rôle de l’utilisateur
    private string GenerateJWTToken(Users user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
        );
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Admin ? "Admin" : "User"),
        };

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Issuer"],
            claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// Modèle utilisé lors du login
public class LoginModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

// Modèle utilisé pour l’inscription
public class RegisterModel
{
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public bool Admin { get; set; }
}

// Modèle utilisé pour la mise à jour du profil utilisateur
public class UpdateUserModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Password { get; set; }
    public bool? Admin { get; set; }
}
