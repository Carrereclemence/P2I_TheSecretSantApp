using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

// Attribut personnalisé qui limite l’accès à un contrôleur ou une méthode aux administrateurs uniquement
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AdminAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    // Méthode exécutée automatiquement avant l'action ciblée pour vérifier les droits
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // Vérifie que l'utilisateur est authentifié (connecté avec un token JWT valide)
        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult(); // 401 Unauthorized si l’utilisateur n’est pas connecté
            return;
        }

        // Vérifie si l'utilisateur a bien le rôle "Admin"
        if (!user.IsInRole("Admin"))
        {
            context.Result = new ForbidResult(); // 403 Forbidden si l’utilisateur n’est pas admin
        }
    }
}
