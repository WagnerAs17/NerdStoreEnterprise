using Microsoft.Extensions.Options;
using NSE.BFF.Compras.Extensions;
using NSE.BFF.Compras.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.BFF.Compras.Services
{
    public interface IClienteService
    {
        Task<EnderecoDTO> ObterEndereco();
    }
    public class ClienteService : Service, IClienteService
    {
        private readonly HttpClient _http;

        public ClienteService(HttpClient http, IOptions<AppServicesSettings> settings)
        {
            _http = http;
            _http.BaseAddress = new Uri(settings.Value.ClienteUrl);
        }

        public async Task<EnderecoDTO> ObterEndereco()
        {
            var response = await _http.GetAsync("/cliente/endereco");

            if (!TratarErrosResponse(response)) return null;

            return await DeserializarObjetoResponse<EnderecoDTO>(response);
        }

    }
}
