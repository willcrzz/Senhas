using System;

namespace Senhas.Models.Entities
{
    public class AuditoriaSistema
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public string? UsuarioLogin { get; set; }
       
        public DateTime DataHora { get; set; }

        public string Ip { get; set; }

        public string? Hostname { get; set; }

        public string Navegador { get; set; }

        
        public string Acao { get; set; }          // Ex: LOGIN, LOGOUT, NOVA SENHA, CHAMAR SENHA
        public string Entidade { get; set; }      // Ex: Senha, Usuario, Guiche
        public int? EntidadeId { get; set; }      // Ex: Id da senha criada/modificada
    }


}