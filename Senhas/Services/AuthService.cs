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

    public async Task<Usuario?> LoginAsync(string usuario, string senha)
    {
        // Tenta no banco
        var userDb = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Nome == usuario && u.Senha == senha);

        if (userDb != null)
            return userDb;

        // Usuário padrão do appsettings
        var usuarioPadrao = _config["Auth:UsuarioPadrao"];
        var senhaPadrao = _config["Auth:SenhaPadrao"];

        if (usuario == usuarioPadrao && senha == senhaPadrao)
        {
            return new Usuario
            {
                Id = 0,
                Nome = usuarioPadrao,
                Senha = senhaPadrao,
                Confirmado = true
            };
        }

        return null;
    }
}