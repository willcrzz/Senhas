using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Senhas.Models.Enums;
using System.Security.Claims;

public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Ignorar tela de login
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

    /// <summary>Id do usuário logado</summary>
    protected int UsuarioId =>
        HttpContext.Session.GetInt32("UsuarioId") ?? 0;

    /// <summary>True se o usuário logado for ADMIN</summary>
    protected bool IsAdmin =>
        HttpContext.Session.GetString("Perfil") == PerfilUsuario.Admin.ToString();

    /// <summary>Nome/login do usuário logado</summary>
    protected string UsuarioNome
    {
        get
        {
            // 1️⃣ Tenta obter da sessão
            var nomeSessao = HttpContext.Session.GetString("UsuarioNome");

            if (!string.IsNullOrEmpty(nomeSessao))
                return nomeSessao;

            // 2️⃣ Tenta obter do Identity/Claims
            var claimName = HttpContext.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(claimName))
                return claimName;

            // 3️⃣ Valor padrão
            return "DESCONHECIDO";
        }
    }
}