using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;
using Senhas.Models.Enums;
using System.Linq;

namespace Senhas.Controllers
{
    public class PainelController : BaseController
    {
        private readonly AppDbContext _context;

        public PainelController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Últimas 5 senhas chamadas ou finalizadas
            var chamadas = _context.Senhas
                .Include(s => s.Guiche)
                .Where(s => s.Status == StatusSenha.EmAtendimento || s.Status == StatusSenha.Finalizada)
                .OrderByDescending(s => s.DataChamada)
                .Take(5)
                .ToList();

            // Calcula tempo médio de espera (em minutos) de todas as senhas chamadas
            var senhasChamadas = _context.Senhas
                .Where(s => s.DataChamada != null)
                .ToList();

            double tempoMedio = 0;

            if (senhasChamadas.Any())
            {
                tempoMedio = senhasChamadas
                    .Average(s => (s.DataChamada.Value - s.DataCriacao).TotalMinutes);
            }

            ViewBag.TempoMedioEspera = $"{Math.Round(tempoMedio)} minutos";


            return View("~/Views/Painel/Index.cshtml", chamadas);
        }
    }
}