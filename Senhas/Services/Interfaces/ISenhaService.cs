using Senhas.Models.Entities;

namespace Senhas.Services.Interfaces
{
    public interface ISenhaService
    {
        Senha GerarSenha(int tipoSenhaId);
        Senha? ChamarProximaSenha(int guicheId);
    }

}
