using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;

public class AdminController : BaseController
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        // Se não for admin → bloqueia
        if (!IsAdmin)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
    }


    // Lista todos usuários e guichês
    public async Task<IActionResult> Index()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.UsuarioGuiches)
            .ThenInclude(ug => ug.Guiche)
            .ToListAsync();

        var guiches = await _context.Guiches.ToListAsync();

        ViewBag.Guiches = guiches;
        return View(usuarios);
    }

    // Atualiza guichês de um usuário
    [HttpPost]
    public async Task<IActionResult> AtualizarGuiches(int usuarioId, List<int> guichesSelecionados)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.UsuarioGuiches)
            .FirstOrDefaultAsync(u => u.Id == usuarioId);

        if (usuario == null)
            return NotFound();

        // Remove todos guichês antigos
        _context.UsuariosGuiches.RemoveRange(usuario.UsuarioGuiches);

        // Adiciona os selecionados
        foreach (var guicheId in guichesSelecionados)
        {
            usuario.UsuarioGuiches.Add(new UsuarioGuiche
            {
                UsuarioId = usuarioId,
                GuicheId = guicheId
            });
        }

        await _context.SaveChangesAsync();
        TempData["Sucesso"] = "Guichês atualizados!";
        return RedirectToAction("Index");
    }
}