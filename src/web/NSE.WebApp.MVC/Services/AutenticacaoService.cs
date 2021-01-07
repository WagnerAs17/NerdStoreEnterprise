using Microsoft.Extensions.Options;
using NSE.Core.Communication;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public class AutenticacaoService : Service, IAutenticacaoService
    {
        private readonly HttpClient httpClient;
        public AutenticacaoService
        (
            HttpClient httpClient,
            IOptions<AppSettings> settings
        )
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri(settings.Value.AutenticacaoUrl);
        }

        public async Task<UsuarioRepostaLogin> Login(UsuarioLogin usuarioLogin)
        {
            var loginContent = ObterDado(usuarioLogin);

            var response = await httpClient.PostAsync("/api/identidade/autenticar", loginContent);

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRepostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }

            return await DeserializarObjetoResponse<UsuarioRepostaLogin>(response);
        }

        public async Task<UsuarioRepostaLogin> Registro(UsuarioRegistro usuarioRegistro)
        {
            var registroContent = ObterDado(usuarioRegistro);

            var response = await this.httpClient.PostAsync("/api/identidade/nova-conta", registroContent);

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRepostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }

            return await DeserializarObjetoResponse<UsuarioRepostaLogin>(response);
        }
    }
}
