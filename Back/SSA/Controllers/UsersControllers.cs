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
    private readonly UserContext _context;
    private readonly IConfiguration _configuration;

    // ✅ Constructeur unique
    public UsersControllers(UserContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // 🔐 GET: api/users (Nécessite un token JWT)
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        var userList = users.Select(u => new
        {
            Id = u.Id,
            UserName = u.UserName,
            FirstName = u.FirstName,
            LastName = u.LastName,
        });

        return Ok(userList);
    }

    // 🔍 GET: api/users/2
    [HttpGet("GET")]
    [Authorize]
    public async Task<ActionResult<Users>> GetItem(int id)
    {
        var user = await _context.Users.SingleOrDefaultAsync(t => t.Id == id);
        if (user == null)
            return NotFound();
        return user;
    }

    // ➕ POST: api/users (Créer un nouvel utilisateur)
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Users>> PostItem(Users item)
    {
        _context.Users.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
    }

    // ✏️ PUT: api/users (Modifier un utilisateur)
    [HttpPut("PUT")]
    [Authorize]
    public async Task<IActionResult> PutItem(int id, Users item)
    {
        if (id != item.Id)
            return BadRequest();

        _context.Entry(item).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Users.Any(m => m.Id == id))
                return NotFound();
            else
                throw;
        }
        return NoContent();
    }

    // 🗑 DELETE: api/users (Supprimer un utilisateur)
    [HttpDelete("DELETE")]
    [Authorize]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _context.Users.FindAsync(id);
        if (item == null)
            return NotFound();

        _context.Users.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
     public class AuthResponse
    {
        public string Token { get; set; }
        public string Message { get; set; }
    }

    [HttpPost("login")]
    [AllowAnonymous] 

    public IActionResult Login([FromBody] LoginModel model)
    {
        var user = _context.Users.SingleOrDefault(u =>
            u.UserName == model.UserName && u.Password == model.Password
        );

        if (user == null)
        {
            return Unauthorized(
                new AuthResponse { Message = "Nom d'utilisateur ou mot de passe incorrect." }
            );
        }

        // Génération du token
        var tokenString = GenerateJWTToken(user.UserName);
        return Ok(new AuthResponse { Token = tokenString, Message = "Connexion réussie !" });
    }

    private string GenerateJWTToken(string username)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
        );
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "User"),
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

    /*public IActionResult Login([FromBody] LoginModel model)
    {
        var user = _context.Users.SingleOrDefault(u =>
            u.UserName == model.UserName && u.Password == model.Password
        );

        if (user == null)
        {
            return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect." });
        }

        // Génération du token JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Admin ? "Admin" : "User"),
                }
            ),
            Expires = DateTime.UtcNow.AddHours(1), // Expiration en 1 heure
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { token = tokenHandler.WriteToken(token) });
    }*/

    [HttpOptions]
    public IActionResult Options()
    {
        return Ok();
    }
}

// ✅ Modèle pour la connexion (LoginModel)
public class LoginModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
}
