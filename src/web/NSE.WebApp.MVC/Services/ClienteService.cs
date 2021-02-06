using Microsoft.Extensions.Options;
using NSE.Core.Communication;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public class ClienteService : Service, IClienteService
    {
        private readonly HttpClient _http;

        public ClienteService(HttpClient http, IOptions<AppSettings> settings)
        {
            _http = http;
            _http.BaseAddress = new Uri(settings.Value.ClienteUrl);
        }

        public async Task<EnderecoViewModel> ObterEndereco()
        {
            var response = await _http.GetAsync("/cliente/endereco");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<EnderecoViewModel>(response);
        }

        public async Task<ResponseResult> AdicionarEndereco(EnderecoViewModel endereco)
        {
            var stringContent = ObterConteudo(endereco);

            var response = await _http.PostAsync("/cliente/endereco", stringContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornOK();
        }
    }
}
