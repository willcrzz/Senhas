using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;
using Senhas.Models.Enums;

namespace Senhas.Controllers
{
    public class PainelController : Controller
    {
        private readonly AppDbContext _context;

        public PainelController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var chamadas = _context.Senhas
                .Include(s => s.Guiche)
                .Where(s => s.Status == StatusSenha.EmAtendimento || s.Status == StatusSenha.Finalizada)
                .OrderByDescending(s => s.DataChamada)
                .Take(5)
                .ToList();

            return View("~/Views/Painel/Index.cshtml", chamadas);
        }
    }
}