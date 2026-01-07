using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;
using Senhas.Models.Enums;

public class AppDbContext : DbContext
{
    public DbSet<Senha> Senhas { get; set; }
    public DbSet<Guiche> Guiches { get; set; }
    public DbSet<TipoSenha> TiposSenha { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Senha>()
            .Property(s => s.Status)
            .HasConversion<string>();

        base.OnModelCreating(modelBuilder);
    }
}