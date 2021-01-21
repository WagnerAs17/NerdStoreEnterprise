using Microsoft.Extensions.Options;
using NSE.BFF.Compras.Extensions;
using NSE.BFF.Compras.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.BFF.Compras.Services
{
    public interface ICatalogoService
    {
        Task<ItemProdutoDTO> ObterPorId(Guid id);
        Task<IEnumerable<ItemProdutoDTO>> ObterItens(IEnumerable<Guid> produtoIds);
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

        public async Task<IEnumerable<ItemProdutoDTO>> ObterItens(IEnumerable<Guid> produtoIds)
        {
            var idsRequest = string.Join(",", produtoIds);

            var response = await _httpClient.GetAsync($"catalogo/produtos/lista/{idsRequest}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<IEnumerable<ItemProdutoDTO>>(response);
        }
    }
}
