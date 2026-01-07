using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public static class DataSeeder
{
    public static async Task SeedAdminAsync(
        UserManager<IdentityUser> userManager,
        IConfiguration config)
    {
        var adminEmail = config["AdminUser:Email"];
        var adminSenha = config["AdminUser:Senha"];

        var user = await userManager.FindByEmailAsync(adminEmail);
        if (user == null)
        {
            var novoAdm = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(novoAdm, adminSenha);
        }
    }
}