using NSE.BFF.Compras.Models;
using NSE.BFF.Compras.Models.Pedidos;
using NSE.Carrinho.API.Services.gRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.BFF.Compras.Services.gRPC
{
    public interface ICarrinhoGrpcService
    {
        Task<CarrinhoDTO> ObterCarrinho();
    }
    public class CarrinhoGrpcService : ICarrinhoGrpcService
    {
        private readonly CarrinhoCompras.CarrinhoComprasClient _client;

        public CarrinhoGrpcService(CarrinhoCompras.CarrinhoComprasClient client)
        {
            _client = client;
        }

        public async Task<CarrinhoDTO> ObterCarrinho()
        {
            var response = await _client.ObterCarrinhoAsync(new ObterCarrinhoRequest());

            return ParaCarrinhoDTO(response);
        }

        private static CarrinhoDTO ParaCarrinhoDTO(CarrinhoClienteResponse carrinhoResponse)
        {
            var carrinhoDTO = new CarrinhoDTO
            {
                ValorTotal = (decimal)carrinhoResponse.Valortotal,
                Desconto = (decimal)carrinhoResponse.Desconto,
                VoucherUtilizado = carrinhoResponse.Voucherutilizado
            };

            if(carrinhoResponse.Voucher != null)
            {
                carrinhoDTO.Voucher = new VoucherDTO
                {
                    Codigo = carrinhoResponse.Voucher.Codigo,
                    Percentual = (decimal?)carrinhoResponse.Voucher.Percentual,
                    ValorDesconto = (decimal?)carrinhoResponse.Voucher.Valordesconto,
                    TipoDesconto = carrinhoResponse.Voucher.Tipodesconto
                };
            }

            foreach (var item in carrinhoResponse.Itens)
            {
                carrinhoDTO.Itens.Add(new ItemCarrinhoDTO
                {
                    Imagem = item.Imagem,
                    Nome = item.Nome,
                    ProdutoId = Guid.Parse(item.Produtoid),
                    Quantidade = item.Quantidade,
                    Valor = (decimal)item.Valor
                });
            }

            return carrinhoDTO;
        }
    }
}
