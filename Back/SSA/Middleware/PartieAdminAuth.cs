using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

[AttributeUsage(AttributeTargets.Method)]
public class PartieChefAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var serviceProvider = context.HttpContext.RequestServices;
        var dbContext = serviceProvider.GetRequiredService<PartieContext>();

        if (!int.TryParse((string)context.RouteData.Values["id"], out int partieId))
        {
            context.Result = new BadRequestObjectResult(new { message = "ID de partie invalide." });
            return;
        }

        var partie = dbContext.Parties.Include(p => p.Chef).FirstOrDefault(p => p.Id == partieId);

        if (partie == null)
        {
            context.Result = new NotFoundObjectResult(new { message = "Partie non trouvée." });
            return;
        }

        var username = user.Identity.Name;
        if (partie.Chef.UserName != username)
        {
            context.Result = new ForbidResult();
        }
    }
}
