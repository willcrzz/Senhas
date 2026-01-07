using Senhas.Models.Entities;
using Senhas.Models.Enums;
using Senhas.Services.Interfaces;

public class SenhaService : ISenhaService
{
    private readonly AppDbContext _context;

    public SenhaService(AppDbContext context)
    {
        _context = context;
    }

    public Senha GerarSenha(int tipoSenhaId)
    {
        var tipo = _context.TiposSenha.Find(tipoSenhaId)!;

        var ultima = _context.Senhas
            .Where(s => s.TipoSenhaId == tipoSenhaId)
            .OrderByDescending(s => s.Id)
            .FirstOrDefault();

        int numero = ultima == null
            ? 1
            : int.Parse(ultima.Codigo.Substring(1)) + 1;

        var senha = new Senha
        {
            Codigo = $"{tipo.Prefixo}{numero:000}",
            TipoSenhaId = tipoSenhaId
        };

        _context.Senhas.Add(senha);
        _context.SaveChanges();

        return senha;
    }

    public Senha? ChamarProximaSenha(int guicheId)
    {
        var senha = _context.Senhas
            .Where(s => s.Status == StatusSenha.Aguardando)
            .OrderByDescending(s => s.TipoSenha.Prioridade)
            .ThenBy(s => s.DataCriacao)
            .FirstOrDefault();

        if (senha == null) return null;

        senha.Status = StatusSenha.EmAtendimento;
        senha.GuicheId = guicheId;
        senha.DataChamada = DateTime.Now;

        _context.SaveChanges();
        return senha;
    }
}