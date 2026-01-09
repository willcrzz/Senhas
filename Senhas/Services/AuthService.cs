using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<Usuario?> LoginAsync(string email, string senha)
    {
        // Normaliza email
        var emailLower = email?.Trim().ToLower();

        // Busca no banco por email e senha
        var userDb = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == emailLower && u.Senha == senha);

        if (userDb != null)
            return userDb;

        // Usuário administrador padrão via appsettings
        var usuarioPadrao = _config["Auth:UsuarioPadrao"];
        var senhaPadrao = _config["Auth:SenhaPadrao"];

        if (emailLower == usuarioPadrao?.ToLower() && senha == senhaPadrao)
        {
            return new Usuario
            {
                Id = 0,
                Nome = usuarioPadrao,
                Sobrenome = "Padrão",
                Email = usuarioPadrao,
                Confirmado = true
            };
        }

        return null;
    }
}