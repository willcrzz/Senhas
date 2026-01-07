using Senhas.Models.Entities;

namespace Senhas.Models.Entities
{
    public class Guiche
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public bool Ativo { get; set; } = true;

        public ICollection<UsuarioGuiche> UsuarioGuiches { get; set; } = new List<UsuarioGuiche>();
    }

}
