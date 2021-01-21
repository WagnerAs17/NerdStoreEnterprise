using Microsoft.Extensions.Options;
using NSE.Core.Communication;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public class ComprasBffService : Service, IComprasBffService
    {
        private readonly HttpClient _httpClient;
        public ComprasBffService
        (
            HttpClient httpClient,
            IOptions<AppSettings> settings
        )
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.ComprasBffUrl);
        }
        
        public async Task<CarrinhoViewModel> ObterCarrinho()
        {
            var response = await _httpClient.GetAsync($"/compras/carrinho/");

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<CarrinhoViewModel>(response);
        }

        public async Task<int> ObterQuantidadeCarrinho()
        {
            var response = await _httpClient.GetAsync("compras/carrinho-quantidade/");

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<int>(response);
        }

        public async Task<ResponseResult> AdicionarItemCarrinho(ItemCarrinhoViewModel produtoViewModel)
        {
            var itemContent = ObterDado(produtoViewModel);

            var response = await _httpClient.PostAsync($"/compras/carrinho/items/", itemContent);

            if (!TratarErrosResponse(response))
            {
                return await DeserializarObjetoResponse<ResponseResult>(response);
            }

            return RetornOK();
        }

        public async Task<ResponseResult> AtualizarItemCarrinho(Guid produtoId, ItemCarrinhoViewModel produto)
        {
            var itemContent = ObterDado(produto);

            var response = await _httpClient.PutAsync($"compras/carrinho/items/{produtoId}", itemContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornOK();
        }

        public async Task<ResponseResult> RemoverItemCarrinho(Guid produtoId)
        {
            var response = await _httpClient.DeleteAsync($"compras/carrinho/items/{produtoId}");

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornOK();
        }

        public async Task<ResponseResult> AplicarVoucherCarrinho(string voucher)
        {
            var stringContent = ObterDado(voucher);

            var response = await _httpClient.PostAsync("compras/carrinho/aplicar-voucher/", stringContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornOK();
        }

        public PedidoTransacaoViewModel MapearParaPedido(CarrinhoViewModel carrinho, EnderecoViewModel endereco)
        {
            var pedido = new PedidoTransacaoViewModel
            {
                Desconto = carrinho.Desconto,
                Itens = carrinho.Itens,
                ValorTotal = carrinho.ValorTotal,
                VoucherCodigo = carrinho.Voucher != null ? carrinho.Voucher.Codigo : null,
                VoucherUtilizado = carrinho.VoucherUtilizado
            };

            if(endereco != null)
            {
                pedido.Endereco = new EnderecoViewModel
                {
                    Bairro = endereco.Bairro,
                    Cep = endereco.Cep,
                    Cidade = endereco.Cidade,
                    Complemento = endereco.Complemento,
                    Estado = endereco.Estado,
                    Logradouro = endereco.Logradouro,
                    Numero = endereco.Numero
                };
            }


            return pedido;
        }

        public async Task<ResponseResult> FinalizarPedido(PedidoTransacaoViewModel pedidoTransacao)
        {
            var stringContent = ObterDado(pedidoTransacao);

            var response = await _httpClient.PostAsync("/compras/pedido", stringContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornOK();
        }

        public async Task<PedidoViewModel> ObterUltimoPedido()
        {
            var response = await _httpClient.GetAsync("/compras/pedido/ultimo");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<PedidoViewModel>(response);
        }

        public async Task<IEnumerable<PedidoViewModel>> ObterListaPorClienteId()
        {
            var response = await _httpClient.GetAsync("compras/pedido/lista-cliente");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<IEnumerable<PedidoViewModel>>(response);
        }
    }
}
