using Microsoft.Extensions.Options;
using NSE.BFF.Compras.Extensions;
using NSE.BFF.Compras.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.BFF.Compras.Services
{
    public interface ICatalogoService
    {
        Task<ItemProdutoDTO> ObterPorId(Guid id);
    }
    public class CatalogoService : Service, ICatalogoService
    {
        private readonly HttpClient _httpClient;

        public CatalogoService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.CatalogoUrl);
        }

        public async Task<ItemProdutoDTO> ObterPorId(Guid id)
        {
            var response = await _httpClient.GetAsync($"catalogo/produtos/{id}");

            return await DeserializarObjetoResponse<ItemProdutoDTO>(response);
        }
    }
}
