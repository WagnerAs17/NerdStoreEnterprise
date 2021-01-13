using NSE.BFF.Compras.Models.Pedidos;
using System.Collections.Generic;

namespace NSE.BFF.Compras.Models
{
    public class CarrinhoDTO
    {
        public CarrinhoDTO()
        {
            Itens = new List<ItemCarrinhoDTO>();
        }
        public VoucherDTO Voucher { get; set; }
        public bool VoucherUtilizado { get; set; }
        public decimal Desconto { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorDesconto { get; set; }
        public List<ItemCarrinhoDTO> Itens { get; set; }
    }
}
