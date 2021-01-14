using System;

namespace NSE.Pedido.API.Application.DTO
{
    public class PedidoItemDTO
    {
        public Guid PedidoId { get; set; }
        public Guid ProdudoId { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
        public string Imagem { get; set; }


        public static Pedidos.Domain.Pedidos.PedidoItem ParaPedidoItem(PedidoItemDTO pedidoItemDto) 
        {
            return new Pedidos.Domain.Pedidos.PedidoItem
                (
                    pedidoItemDto.ProdudoId, 
                    pedidoItemDto.Nome, 
                    pedidoItemDto.Quantidade, 
                    pedidoItemDto.Valor, 
                    pedidoItemDto.Imagem
                );
        }

    }
}
