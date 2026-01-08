using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;
using Senhas.Models.Enums;
using Senhas.Services;

namespace Senhas.Controllers
{
    public class AtendimentoController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly AuditoriaService _auditoria;

        public AtendimentoController(AppDbContext context, AuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
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


       


        // Tela principal do atendente
        public IActionResult Index()
        {
            var senhaAtual = _context.Senhas
                .Include(s => s.TipoSenha)
                .Include(s => s.Guiche)
                .Where(s => s.Status == StatusSenha.EmAtendimento)
                .OrderByDescending(s => s.DataChamada)
                .FirstOrDefault();

            // Carregando guichês cadastrados
            ViewBag.Guiches = _context.Guiches.ToList();

            return View(senhaAtual);
        }

        // Chamar próxima senha
        [HttpPost]
        public async Task<IActionResult> ChamarProxima()
        {
            var usuarioId = UsuarioId;
            var usuarioNome = UsuarioNome;

            var guichesUsuario = _context.UsuariosGuiches
                .Where(x => x.UsuarioId == usuarioId)
                .Select(x => x.GuicheId)
                .ToList();

            if (!guichesUsuario.Any())
            {
                TempData["Mensagem"] = "Usuário não está vinculado a nenhum guichê!";
                return RedirectToAction("Index");
            }

            var guicheId = guichesUsuario.First();

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

            senha.UsuarioId = UsuarioId; // id do atendente logado
            senha.DataChamada = DateTime.UtcNow;
            senha.Status = StatusSenha.EmAtendimento;
            senha.GuicheId = guicheId;


            try
            {
                _context.SaveChanges();

                await _auditoria.RegistrarAsync(
                    acao: "CHAMAR SENHA",
                    entidade: "Senha",
                    entidadeId: senha.Id
                    
                );
            }
            catch (DbUpdateException ex)
            {
                TempData["Mensagem"] = "Erro ao chamar a senha: " + ex.InnerException?.Message;
            }

            TempData["Mensagem"] = $"Senha {senha.Codigo} chamada no Guichê {guicheId}";
            return RedirectToAction("Index");
        }


        // Finalizar atendimento
        [HttpPost]
        public async Task<IActionResult> Finalizar(int senhaId)
        {
            var usuarioId = UsuarioId;
            var usuarioNome = UsuarioNome;

            var guichesUsuario = _context.UsuariosGuiches
                .Where(x => x.UsuarioId == usuarioId)
                .Select(x => x.GuicheId)
                .ToList();

            if (!guichesUsuario.Any())
            {
                TempData["Mensagem"] = "Usuário não está vinculado a nenhum guichê!";
                return RedirectToAction("Index");
            }

            var guicheId = guichesUsuario.First();



            var senha = _context.Senhas.Find(senhaId);
            if (senha == null)
                return NotFound();

            senha.UsuarioId = UsuarioId; // id do atendente logado
            senha.DataFinalizacao = DateTime.UtcNow;
            senha.Status = StatusSenha.Finalizada;
            senha.GuicheId = guicheId;
            _context.SaveChanges();

            
            await _auditoria.RegistrarAsync(
                acao: "FINALIZAR SENHA",
                entidade: "Senha",
                entidadeId: senha.Id
            );

            return RedirectToAction("Index");
        }

            }
        }
