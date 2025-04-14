using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

[AttributeUsage(AttributeTargets.Method)]
public class PartieChefAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // Vérifie que l'utilisateur est authentifié (connecté)
        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult(); // 401 Unauthorized
            return;
        }

        // Récupère une instance de AppDbContext via le système d'injection de dépendances
        var serviceProvider = context.HttpContext.RequestServices;
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();

        // Récupère l’ID de la partie depuis l’URL (ex: /partie/5/tirage)
        if (!int.TryParse((string)context.RouteData.Values["id"], out int partieId))
        {
            context.Result = new BadRequestObjectResult(new { message = "ID de partie invalide." });
            return;
        }

        // Récupère la partie correspondante avec les infos du chef
        var partie = dbContext.Parties.Include(p => p.Chef).FirstOrDefault(p => p.Id == partieId);

        // Si la partie n’existe pas, on renvoie une erreur 404
        if (partie == null)
        {
            context.Result = new NotFoundObjectResult(new { message = "Partie non trouvée." });
            return;
        }

        var username = user.Identity.Name;

        // Vérifie que l'utilisateur connecté est bien le chef de la partie
        if (partie.Chef.UserName != username)
        {
            context.Result = new ForbidResult(); // 403 Forbidden
        }
    }
}
