using System;
using System.Collections.Generic;

namespace NSE.Pedido.API.Application.DTO
{
    public class PedidoDTO
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public int Codigo { get; set; }
        public int Status { get; set; }
        public DateTime Data { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal Desconto { get; set; }
        public string VoucherCodigo { get; set; }
        public bool VoucherUtilizado { get; set; }
        public List<PedidoItemDTO> PedidoItems { get; set; }
        public EnderecoDTO Endereco { get; set; }
        public static PedidoDTO ParaPedidoDTO(Pedidos.Domain.Pedidos.Pedido pedido)
        {
            var pedidoDTO = new PedidoDTO
            {
                Id = pedido.Id,
                Codigo = pedido.Codido,
                Data = pedido.DataCadastro,
                Desconto = pedido.Desconto,
                Status = (int)pedido.PedidoStatus,
                ValorTotal = pedido.ValorTotal,
                VoucherUtilizado = pedido.VoucherUtilizado,
                PedidoItems = new List<PedidoItemDTO>()
            };

            foreach (var item in pedido.PedidoItems)
            {
                pedidoDTO.PedidoItems.Add(new PedidoItemDTO
                {
                    Nome = item.ProdutoNome,
                    Imagem = item.ProdutoImagem,
                    PedidoId = item.PedidoId,
                    ProdudoId = item.ProdudoId,
                    Quantidade = item.Quantidade,
                    Valor = item.ValorUnitario
                });
            }

            pedidoDTO.Endereco = new EnderecoDTO
            {
                Bairro = pedido.Endereco.Bairro,
                Cep = pedido.Endereco.Cep,
                Cidade = pedido.Endereco.Cidade,
                Complemento = pedido.Endereco.Complemento,
                Estado = pedido.Endereco.Estado,
                Logradouro = pedido.Endereco.Logradouro,
                Numero = pedido.Endereco.Numero
            };

            return pedidoDTO;
        }
    }
}
