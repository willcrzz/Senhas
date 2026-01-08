using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;
using Senhas.Models.Enums;

namespace Senhas.Controllers
{
    public class SenhasController : BaseController
    {
        private readonly AppDbContext _context;

        public SenhasController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Gerar()
        {
            var tipos = _context.TiposSenha
                .AsNoTracking()
                .OrderBy(t => t.Prioridade)
                .ToList();

            return View(tipos);
        }

        [HttpPost]
        public IActionResult Gerar(int tipoSenhaId)
        {
            var tipo = _context.TiposSenha.Find(tipoSenhaId);
            if (tipo == null)
                return NotFound();

            var ultimaSenha = _context.Senhas
                .Where(s => s.TipoSenhaId == tipoSenhaId)
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();

            int numero = ultimaSenha == null
                ? 1
                : int.Parse(ultimaSenha.Codigo.Substring(1)) + 1;

            var senha = new Senha
            {
                Codigo = $"{tipo.Prefixo}{numero:000}",
                TipoSenhaId = tipoSenhaId,
                Status = StatusSenha.Aguardando,
                DataCriacao = DateTime.UtcNow
            };

                     
            _context.Senhas.Add(senha);
            _context.SaveChanges();

            return RedirectToAction("Gerada", new { id = senha.Id });
        }

        public IActionResult Gerada(int id)
        {
            var senha = _context.Senhas
                .Include(s => s.TipoSenha)
                .FirstOrDefault(s => s.Id == id);

            if (senha == null)
                return NotFound();

            return View(senha);
        }
    }
}
