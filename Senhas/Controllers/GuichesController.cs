using Microsoft.AspNetCore.Mvc;

namespace Senhas.Controllers
{
    public class GuichesController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

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

    }
}
