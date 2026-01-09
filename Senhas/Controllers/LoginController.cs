using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senhas.Models.Entities;
using System.Net;
using System.Net.Mail;

public class LoginController : Controller
{
    private readonly AuthService _auth;
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public LoginController(AuthService auth, AppDbContext context, IConfiguration config)
    {
        _auth = auth;
        _context = context;
        _config = config;
    }

    [HttpGet]
    public IActionResult Index() => View();

    // LOGIN
    [HttpPost]
    public async Task<IActionResult> Login(string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            TempData["Erro"] = "Preencha o e-mail e a senha.";
            return RedirectToAction("Index");
        }

        email = email.Trim().ToLower();

        var user = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email && u.Senha == senha);

        if (user == null)
        {
            TempData["Erro"] = "Usuário ou senha inválidos.";
            return RedirectToAction("Index");
        }

        if (!user.Confirmado)
        {
            TempData["Erro"] = "Confirme seu e-mail antes de logar.";
            return RedirectToAction("Index");
        }

        // Busca guichê vinculado
        var guicheUser = await _context.UsuariosGuiches
            .Where(x => x.UsuarioId == user.Id)
            .Select(x => x.GuicheId)
            .FirstOrDefaultAsync();

        var perfil = user.Perfil.ToString().ToLower();

        // BLOQUEAR atendente sem guichê
        if (perfil != "admin" && guicheUser == 0)
        {
            TempData["Erro"] = "Usuário não possui guichê vinculado! Contate o administrador.";
            return RedirectToAction("Index");
        }

        // Salvar sessão
        HttpContext.Session.SetInt32("UsuarioId", user.Id);
        HttpContext.Session.SetString("UsuarioNome", $"{user.Nome} {user.Sobrenome}");
        HttpContext.Session.SetString("Perfil", user.Perfil.ToString());

        if (guicheUser > 0)
            HttpContext.Session.SetInt32("GuicheId", guicheUser);

        // Captura IP e navegador
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconhecido";
        var navegador = Request.Headers["User-Agent"].ToString();

        // Gravar auditoria
        _context.AuditoriaSistema.Add(new AuditoriaSistema
        {
            UsuarioId = user.Id,
            UsuarioLogin = user.Nome,
            DataHora = DateTime.UtcNow,
            Ip = ip,
            Navegador = navegador,
            Acao = "LOGIN",
            Entidade = "Usuario",
            EntidadeId = user.Id
        });

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    // LOGOUT
    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    // REGISTRAR
    [HttpPost]
    public async Task<IActionResult> Registrar(
        string nome,
        string sobrenome,
        string cpf,
        string email,
        string usuario,
        string senha,
        string confirmarSenha)
    {
        if (senha != confirmarSenha)
        {
            TempData["Erro"] = "As senhas não coincidem!";
            return RedirectToAction("Index");
        }

        email = email.Trim().ToLower();

        if (await _context.Usuarios.AnyAsync(u => u.Email == email))
        {
            TempData["Erro"] = "E-mail já existe!";
            return RedirectToAction("Index");
        }

        if (await _context.Usuarios.AnyAsync(u => u.Cpf == cpf))
        {
            TempData["Erro"] = "CPF já existe!";
            return RedirectToAction("Index");
        }

        var token = Guid.NewGuid().ToString();

        var novo = new Usuario
        {
            Nome = nome,
            Sobrenome = sobrenome,
            Email = email,
            Cpf = cpf,
            Senha = senha,
            Confirmado = false,
            TokenConfirmacao = token
        };

        _context.Usuarios.Add(novo);
        await _context.SaveChangesAsync();

        var linkConfirmacao = Url.Action("ConfirmarEmail", "Login", new { token }, Request.Scheme);
        await EnviarEmailAsync(email, "Confirme seu cadastro", $"Clique para confirmar: <a href='{linkConfirmacao}'>Confirmar</a>");

        TempData["Sucesso"] = "Cadastro realizado! Verifique seu e-mail para confirmar.";
        return RedirectToAction("Index");
    }

    // CONFIRMAR EMAIL
    [HttpGet]
    public async Task<IActionResult> ConfirmarEmail(string token)
    {
        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenConfirmacao == token);
        if (user == null)
        {
            TempData["Erro"] = "Token inválido.";
            return RedirectToAction("Index");
        }

        user.Confirmado = true;
        user.TokenConfirmacao = null;
        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "E-mail confirmado! Agora você pode logar.";
        return RedirectToAction("Index");
    }

    // ESQUECI SENHA
    [HttpPost]
    public async Task<IActionResult> EsqueciSenha(string email)
    {
        email = email.Trim().ToLower();

        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            TempData["Erro"] = "E-mail não encontrado.";
            return RedirectToAction("Index");
        }

        var token = Guid.NewGuid().ToString();
        user.TokenConfirmacao = token;
        await _context.SaveChangesAsync();

        var linkRedefinir = Url.Action("RedefinirSenha", "Login", new { token }, Request.Scheme);
        await EnviarEmailAsync(email, "Redefinir senha", $"Clique para redefinir: <a href='{linkRedefinir}'>Redefinir</a>");

        TempData["Sucesso"] = "Link enviado para seu e-mail.";
        return RedirectToAction("Index");
    }

    // FORMULARIO REDIFINIR SENHA
    [HttpGet]
    public IActionResult RedefinirSenha(string token)
    {
        ViewBag.Token = token;
        return View();
    }

    // CONFIRMAR NOVA SENHA
    [HttpPost]
    public async Task<IActionResult> RedefinirSenhaConfirmar(string token, string novaSenha, string confirmarSenha)
    {
        if (novaSenha != confirmarSenha)
        {
            TempData["Erro"] = "As senhas não coincidem!";
            return RedirectToAction("RedefinirSenha", new { token });
        }

        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenConfirmacao == token);
        if (user == null)
        {
            TempData["Erro"] = "Token inválido.";
            return RedirectToAction("Index");
        }

        user.Senha = novaSenha;
        user.TokenConfirmacao = null;
        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Senha redefinida com sucesso!";
        return RedirectToAction("Index");
    }

    // ENVIO DE E-MAIL
    private async Task EnviarEmailAsync(string para, string assunto, string mensagem)
    {
        var smtpHost = _config["Email:SmtpHost"];
        var smtpPort = int.Parse(_config["Email:SmtpPort"]);
        var smtpUser = _config["Email:SmtpUser"];
        var smtpPass = _config["Email:SmtpPass"];

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var mail = new MailMessage(smtpUser, para, assunto, mensagem)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(mail);
    }
}
