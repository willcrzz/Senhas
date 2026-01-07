using Senhas.Models.Entities;
namespace Senhas.Models.Entities
{
    public class TipoSenha
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Prefixo { get; set; } = null!;
        public int Prioridade { get; set; }
    }

}
