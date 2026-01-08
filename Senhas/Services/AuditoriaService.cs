using Microsoft.AspNetCore.Http;
using Senhas.Models.Entities;
using System;
using System.Threading.Tasks;

namespace Senhas.Services
{
    public class AuditoriaService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BaseControllerUserGetter _userGetter;

        public AuditoriaService(AppDbContext context, IHttpContextAccessor accessor, BaseControllerUserGetter userGetter)
        {
            _context = context;
            _httpContextAccessor = accessor;
            _userGetter = userGetter;
        }

        public async Task RegistrarAsync(string acao, string entidade = null, int? entidadeId = null)
        {

            var http = _httpContextAccessor.HttpContext;

            var auditoria = new AuditoriaSistema
            {
                UsuarioId = _userGetter.UsuarioId,
                UsuarioLogin = _userGetter.UsuarioLogin,
                DataHora = DateTime.UtcNow,
                Ip = http?.Connection?.RemoteIpAddress?.ToString(),
                Navegador = http?.Request?.Headers["User-Agent"].ToString(),
                Acao = acao,
                Entidade = entidade,
                EntidadeId = entidadeId
            };

            _context.AuditoriaSistema.Add(auditoria);
            await _context.SaveChangesAsync();
        }
    }

    // Esse cara apenas pega as infos do usuário do BaseController
    // sem precisar injetar BaseController direto.
    public class BaseControllerUserGetter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseControllerUserGetter(IHttpContextAccessor accessor)
        {
            _httpContextAccessor = accessor;
        }

        public int UsuarioId =>
            int.Parse(_httpContextAccessor.HttpContext.Session.GetInt32("UsuarioId")?.ToString() ?? "0");

        public string UsuarioLogin =>
            _httpContextAccessor.HttpContext.Session.GetString("UsuarioNome") ?? "DESCONHECIDO";

    }
}
