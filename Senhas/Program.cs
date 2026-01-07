using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Banco PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Criar o banco e aplicar seeds básicos
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate(); // cria banco se não existir

    if (!context.TiposSenha.Any())
    {
        context.TiposSenha.AddRange(
            new TipoSenha { Nome = "Normal", Prefixo = "N", Prioridade = 1 },
            new TipoSenha { Nome = "Prioritária", Prefixo = "P", Prioridade = 3 },
            new TipoSenha { Nome = "Preferencial", Prefixo = "S", Prioridade = 2 }
        );

        context.SaveChanges();
    }

    if (!context.Guiches.Any())
    {
        context.Guiches.AddRange(
            new Guiche { Nome = "Guichê 01" },
            new Guiche { Nome = "Guichê 02" },
            new Guiche { Nome = "Guichê 03" }
        );
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();