using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;
using Senhas.Models.Enums;
using Senhas.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditoriaService>();
builder.Services.AddScoped<BaseControllerUserGetter>();
// Banco PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<AuthService>();
var app = builder.Build();

// Criar o banco e aplicar seeds básicos
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate(); // cria banco se não existir

    // TIPOS SENHA
    if (!context.TiposSenha.Any())
    {
        context.TiposSenha.AddRange(
            new TipoSenha { Nome = "Normal", Prefixo = "N", Prioridade = 1 },
            new TipoSenha { Nome = "Prioritária", Prefixo = "P", Prioridade = 3 },
            new TipoSenha { Nome = "Preferencial", Prefixo = "S", Prioridade = 2 }
        );
        context.SaveChanges();
    }

    // GUICHÊS
    if (!context.Guiches.Any())
    {
        context.Guiches.AddRange(
            new Guiche { Nome = "Guichê 01" },
            new Guiche { Nome = "Guichê 02" },
            new Guiche { Nome = "Guichê 03" }
        );
        context.SaveChanges();
    }

    // ADMIN SEED DO APPS SETTINGS
    if (!context.Usuarios.Any(u => u.Perfil == PerfilUsuario.Admin))
    {
        var adminCfg = builder.Configuration.GetSection("AdminAccount");

        var admin = new Usuario
        {
            Nome = adminCfg["Nome"] ?? "Administrador",
            Sobrenome = adminCfg["Sobrenome"] ?? "Sistema",
            Senha = adminCfg["Password"] ?? "123",
            Email = adminCfg["Email"] ?? "admin@sistema.local",
            Cpf = adminCfg["Cpf"] ?? "00000000000",
            Confirmado = true,
            Perfil = PerfilUsuario.Admin
        };

        context.Usuarios.Add(admin);
        context.SaveChanges();
    }


}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();