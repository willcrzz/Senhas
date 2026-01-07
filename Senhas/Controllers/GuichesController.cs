using Microsoft.AspNetCore.Mvc;

namespace Senhas.Controllers
{
    public class GuichesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
