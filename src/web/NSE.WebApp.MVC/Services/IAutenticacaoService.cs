using NSE.WebApi.Core.Usuario;
using NSE.WebApp.MVC.Models;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public interface IAutenticacaoService
    {
        Task<UsuarioRepostaLogin> Login(UsuarioLogin usuarioLogin);
        Task<UsuarioRepostaLogin> Registro(UsuarioRegistro usuarioRegistro);
        Task RealizarLogin(UsuarioRepostaLogin resposta);
        Task Logout();
        bool TokenExpirado();
        Task<bool> RefreshTokenValido();
    }
}
