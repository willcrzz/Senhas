using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;
using Senhas.Models.Enums;

public class AppDbContext : DbContext
{
    public DbSet<Senha> Senhas { get; set; }
    public DbSet<Guiche> Guiches { get; set; }
    public DbSet<TipoSenha> TiposSenha { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    public DbSet<UsuarioGuiche> UsuariosGuiches { get; set; }
    public DbSet<AuditoriaSistema> AuditoriaSistema { get; set; }



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

        modelBuilder.Entity<UsuarioGuiche>()
            .HasKey(x => new { x.UsuarioId, x.GuicheId });

        modelBuilder.Entity<UsuarioGuiche>()
            .HasOne(x => x.Usuario)
            .WithMany(x => x.UsuarioGuiches)
            .HasForeignKey(x => x.UsuarioId);

        modelBuilder.Entity<UsuarioGuiche>()
            .HasOne(x => x.Guiche)
            .WithMany(x => x.UsuarioGuiches)
            .HasForeignKey(x => x.GuicheId);

        modelBuilder.Entity<Senha>()
            .HasOne(s => s.Usuario)
            .WithMany()
            .HasForeignKey(s => s.UsuarioId);

        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties()
                         .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
            {
                var converterNonNullable =
                    new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                        v => v, // salva sem alterar
                        v => DateTime.SpecifyKind(v, DateTimeKind.Local) // lê como Local
                    );

                var converterNullable =
                    new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
                        v => v,
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Local) : v
                    );

                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(converterNonNullable);
                else
                    property.SetValueConverter(converterNullable);
            }
        }
       
    }


}