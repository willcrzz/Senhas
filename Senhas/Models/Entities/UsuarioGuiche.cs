namespace Senhas.Models.Entities
{
    public class UsuarioGuiche
    {
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int GuicheId { get; set; }
        public Guiche Guiche { get; set; }
    }
}
