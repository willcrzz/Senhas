using Senhas.Models.Enums;
using System.ComponentModel.DataAnnotations;



namespace Senhas.Models.Entities
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required] public string Nome { get; set; } = null!;

        [Required] public string Sobrenome { get; set; } = null!;

        
        [Required] [EmailAddress] public string Email { get; set; } = null!;

        [Required]
        [RegularExpression(@"\d{11}", ErrorMessage = "CPF deve ter 11 números")]
        public string Cpf { get; set; } = null!;

        [Required]
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        public string Senha { get; set; } = null!;

        public bool Confirmado { get; set; } = false;

        public string? TokenConfirmacao { get; set; }

        public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Normal;


        // Guichês atribuídos
        public List<UsuarioGuiche> UsuarioGuiches { get; set; }

    }



}

