using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Senhas.Models.Enums;

public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Ignorar login
        if (context.RouteData.Values["controller"]?.ToString() == "Login")
        {
            base.OnActionExecuting(context);
            return;
        }

        // Se não tem sessão, redireciona
        if (!context.HttpContext.Session.Keys.Contains("UsuarioId"))
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return;
        }

        base.OnActionExecuting(context);
    }

    /// <summary>
    /// ID do usuário logado (0 se não estiver logado)
    /// </summary>
    protected int UsuarioId =>
        HttpContext.Session.GetInt32("UsuarioId") ?? 0;

    /// <summary>
    /// Retorna TRUE se o usuário logado for ADMIN
    /// </summary>
    protected bool IsAdmin =>
        HttpContext.Session.GetString("Perfil") == PerfilUsuario.Admin.ToString();

}