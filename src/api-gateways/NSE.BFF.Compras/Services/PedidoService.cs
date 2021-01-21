﻿using Microsoft.Extensions.Options;
using NSE.BFF.Compras.Extensions;
using NSE.BFF.Compras.Models.Pedidos;
using NSE.Core.Communication;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.BFF.Compras.Services
{
    public interface IPedidoService
    {
        Task<VoucherDTO> ObterVoucherPorCodigo(string codigo);
        Task<ResponseResult> FinalizarPedido(PedidoDTO pedido);
        Task<PedidoDTO> ObterUltimoPedido();
        Task<IEnumerable<PedidoDTO>> ObterListaPedidoPorClienteId();
    }
    public class PedidoService : Service, IPedidoService
    {
        private readonly HttpClient _httpClient;

        public PedidoService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.PedidoUrl);
        }

        public async Task<VoucherDTO> ObterVoucherPorCodigo(string codigo)
        {
            var response = await _httpClient.GetAsync($"/voucher/{codigo}/");

            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<VoucherDTO>(response);
        }

        public async Task<PedidoDTO> ObterUltimoPedido()
        {
            var response = await _httpClient.GetAsync("pedido/ultimo");

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<PedidoDTO>(response);
        }

        public async Task<IEnumerable<PedidoDTO>> ObterListaPedidoPorClienteId()
        {
            var response = await _httpClient.GetAsync("pedido/lista-cliente");

            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<IEnumerable<PedidoDTO>>(response);
        }

        public async Task<ResponseResult> FinalizarPedido(PedidoDTO pedido)
        {
            var stringContent = ObterDado(pedido);

            var response = await _httpClient.PostAsync("pedido", stringContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return ReturnOK();
        }
    }
}
