using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;
using Senhas.Models.Enums;

namespace Senhas.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // Página principal
        public IActionResult Index()
        {
            // Para popular dropdowns de filtros
            ViewBag.Usuarios = _context.Usuarios.ToList();
            ViewBag.Guiches = _context.Guiches.ToList();

            return View();
        }

        // Endpoint AJAX para trazer dados filtrados
        [HttpGet]
        public IActionResult ObterDados(DateTime? inicio, DateTime? fim, int? usuarioId, int? guicheId)
        {
            var query = _context.Senhas
                .Include(s => s.Usuario)
                .Include(s => s.Guiche)
                .Where(s => s.DataChamada != null);

            if (inicio.HasValue)
            {
                // Converte para UTC
                var inicioUtc = DateTime.SpecifyKind(inicio.Value.Date, DateTimeKind.Utc);
                query = query.Where(s => s.DataChamada >= inicioUtc);
            }

            if (fim.HasValue)
            {
                // Converte para UTC e adiciona o último segundo do dia
                var fimUtc = DateTime.SpecifyKind(fim.Value.Date.AddDays(1).AddSeconds(-1), DateTimeKind.Utc);
                query = query.Where(s => s.DataChamada <= fimUtc);
            }

            if (usuarioId.HasValue)
                query = query.Where(s => s.Usuario != null && s.Usuario.Id == usuarioId.Value);

            if (guicheId.HasValue)
                query = query.Where(s => s.Guiche != null && s.Guiche.Id == guicheId.Value);

            var dados = query.ToList()
                .GroupBy(s => s.Usuario != null ? s.Usuario.Nome + " " + s.Usuario.Sobrenome : "Desconhecido")
                .Select(g => new
                {
                    Atendente = g.Key,
                    QuantidadeSenhas = g.Count(),
                    TempoMedioEspera = g.Average(s => (s.DataChamada!.Value - s.DataCriacao).TotalMinutes),
                    TempoMedioAtendimento = g.Average(s => s.DataFinalizacao.HasValue ?
                        (s.DataFinalizacao.Value - s.DataChamada!.Value).TotalMinutes : 0)
                })
                .ToList();

            return Json(dados);
        }

    }
}
