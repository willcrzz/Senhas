using Senhas.Models.Entities;
using Senhas.Models.Enums;

namespace Senhas.Models.Entities
{
    public class Senha
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public int TipoSenhaId { get; set; }
        public TipoSenha TipoSenha { get; set; } = null!;

        public StatusSenha Status { get; set; } = StatusSenha.Aguardando;

        public DateTimeOffset DataCriacao { get; set; }
        public DateTimeOffset? DataChamada { get; set; }
        public DateTimeOffset? DataFinalizacao { get; set; }


        public int? GuicheId { get; set; }
        public Guiche? Guiche { get; set; }

        public int? UsuarioId { get; set; } // atendente
        public Usuario? Usuario { get; set; } // navegação
    }

    


    }

