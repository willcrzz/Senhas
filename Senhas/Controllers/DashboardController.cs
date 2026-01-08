using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;

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
            ViewBag.Usuarios = _context.Usuarios.ToList();
            ViewBag.Guiches = _context.Guiches.ToList();

            return View();
        }

        [HttpGet]
        public IActionResult ObterDados(DateTime? inicio, DateTime? fim, int? usuarioId, int? guicheId)
        {
            var query = _context.Senhas
                .Include(s => s.Usuario)
                .Include(s => s.Guiche)
                .Where(s => s.DataChamada != null);

            // Convertendo datas para UTC 
            if (inicio.HasValue)
            {
                var inicioUtc = DateTime.SpecifyKind(inicio.Value.Date, DateTimeKind.Utc);
                query = query.Where(s => s.DataChamada >= inicioUtc);
            }

            if (fim.HasValue)
            {
                var fimUtc = DateTime.SpecifyKind(fim.Value.Date.AddDays(1).AddSeconds(-1), DateTimeKind.Utc);
                query = query.Where(s => s.DataChamada <= fimUtc);
            }

            // Filtro por atendente
            if (usuarioId.HasValue)
                query = query.Where(s => s.UsuarioId == usuarioId.Value);

            // Filtro por guichê
            if (guicheId.HasValue)
                query = query.Where(s => s.GuicheId == guicheId.Value);

            // Calculando agrupado por atendente
            var dados = query.ToList()
                .GroupBy(s => s.Usuario != null ? $"{s.Usuario.Nome} {s.Usuario.Sobrenome}" : "Desconhecido")
                .Select(g =>
                {
                    var finalizadas = g.Where(x => x.DataFinalizacao.HasValue).ToList();

                    return new
                    {
                        Atendente = g.Key,
                        QuantidadeSenhas = g.Count(),

                        TempoMedioEspera = g.Any()
                            ? g.Average(s => (s.DataChamada!.Value - s.DataCriacao).TotalMinutes)
                            : 0,

                        TempoMedioAtendimento = finalizadas.Any()
                            ? finalizadas.Average(s => (s.DataFinalizacao!.Value - s.DataChamada!.Value).TotalMinutes)
                            : 0
                    };
                })
                .ToList();

            return Json(dados);
        }
    }
}
