using Microsoft.AspNetCore.Mvc;

public class BaseController : Controller
{
    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        var controller = context.Controller as Controller;

        // Ignorar LoginController
        if (context.RouteData.Values["controller"]?.ToString() == "Login")
        {
            base.OnActionExecuting(context);
            return;
        }

        if (!context.HttpContext.Session.Keys.Contains("UsuarioId"))
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return;
        }

        base.OnActionExecuting(context);
    }

    /// <summary>
    /// Retorna o ID do usuário logado gravado na sessão.
    /// </summary>
    protected int UsuarioId()
    {
        var id = HttpContext.Session.GetInt32("UsuarioId");
        return id ?? 0;
    }
}