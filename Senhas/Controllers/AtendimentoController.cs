using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;
using Senhas.Models.Enums;

namespace Senhas.Controllers
{
    public class AtendimentoController : Controller
    {
        private readonly AppDbContext _context;

        public AtendimentoController(AppDbContext context)
        {
            _context = context;
        }

        // Tela principal do atendente
        public IActionResult Index()
        {
            var senhaAtual = _context.Senhas
                .Include(s => s.TipoSenha)
                .Include(s => s.Guiche)
                .Where(s => s.Status == StatusSenha.EmAtendimento)
                .OrderByDescending(s => s.DataChamada)
                .FirstOrDefault();

            ViewBag.Guiches = _context.Guiches.ToList();

            return View(senhaAtual);
        }

        // Chamar próxima senha
        [HttpPost]
        public IActionResult ChamarProxima(int guicheId)
        {
            var senha = _context.Senhas
                .Include(s => s.TipoSenha)
                .Where(s => s.Status == StatusSenha.Aguardando)
                .OrderByDescending(s => s.TipoSenha.Prioridade)
                .ThenBy(s => s.DataCriacao)
                .FirstOrDefault();

            if (senha == null)
            {
                TempData["Mensagem"] = "Nenhuma senha aguardando.";
                return RedirectToAction("Index");
            }

            senha.Status = StatusSenha.EmAtendimento;
            senha.GuicheId = guicheId;
            senha.DataChamada = DateTime.UtcNow;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Finalizar atendimento
        [HttpPost]
        public IActionResult Finalizar(int senhaId)
        {
            var senha = _context.Senhas.Find(senhaId);
            if (senha == null)
                return NotFound();

            senha.Status = StatusSenha.Finalizada;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
