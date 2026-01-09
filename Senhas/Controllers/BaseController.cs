using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Senhas.Models.Enums;

public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.RouteData.Values["controller"]?.ToString();
        var action = context.RouteData.Values["action"]?.ToString();

        //  Ignorar Login + Acesso Negado
        if (controller == "Login" || (controller == "Home" && action == "AcessoNegado"))
        {
            base.OnActionExecuting(context);
            return;
        }

        //  Se NÃO estiver logado, volta ao Login
        if (!context.HttpContext.Session.Keys.Contains("UsuarioId"))
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return;
        }

        base.OnActionExecuting(context);
    }

    // ==============================
    //  PERFIS
    // ==============================

    /// <summary>Id do usuário logado</summary>
    protected int UsuarioId =>
        HttpContext.Session.GetInt32("UsuarioId") ?? 0;

    /// <summary>True se ADMIN</summary>
    protected bool IsAdmin =>
        HttpContext.Session.GetString("Perfil") == PerfilUsuario.Admin.ToString();

    /// <summary>True se USUÁRIO COMUM</summary>
    protected bool IsUsuario =>
        HttpContext.Session.GetString("Perfil") == PerfilUsuario.Usuario.ToString();

    /// <summary>True se NORMAL / GUICHÊ</summary>
    protected bool IsNormal =>
        HttpContext.Session.GetString("Perfil") == PerfilUsuario.Normal.ToString();

    /// <summary>Nome/login do usuário logado</summary>
    protected string UsuarioNome
    {
        get
        {
            var nomeSessao = HttpContext.Session.GetString("UsuarioNome");
            if (!string.IsNullOrEmpty(nomeSessao))
                return nomeSessao;

            var claimName = HttpContext.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(claimName))
                return claimName;

            return "DESCONHECIDO";
        }
    }

    // ==============================
    //  BLOQUEIOS
    // ==============================

    protected bool BloquearSeNaoAdmin(ActionExecutingContext context)
    {
        if (!IsAdmin)
        {
            context.Result = new RedirectToActionResult("AcessoNegado", "Home", null);
            return true;
        }
        return false;
    }

    protected bool BloquearSeNaoUsuario(ActionExecutingContext context)
    {
        if (!IsUsuario)
        {
            context.Result = new RedirectToActionResult("AcessoNegado", "Home", null);
            return true;
        }
        return false;
    }

    protected bool BloquearSeNaoNormal(ActionExecutingContext context)
    {
        if (!IsNormal)
        {
            context.Result = new RedirectToActionResult("AcessoNegado", "Home", null);
            return true;
        }
        return false;
    }
}
