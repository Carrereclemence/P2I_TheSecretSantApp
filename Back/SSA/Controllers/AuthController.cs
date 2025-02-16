using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    // Modèle d'utilisateur
    public class UserLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    // Modèle de réponse avec le token
    public class AuthResponse
    {
        public string Token { get; set; }
        public string Message { get; set; }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin login)
    {
        // Ici, tu devras valider l'utilisateur en base de données
        var user = FakeUserCheck(login.UserName, login.Password);

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
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "User"),
        };

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Simule la vérification d'un utilisateur (à remplacer par une requête à la base de données)
    private static dynamic FakeUserCheck(string username, string password)
    {
        if (username == "test" && password == "password") // Remplace avec ta logique réelle
        {
            return new { UserName = username };
        }
        return null;
    }
}
