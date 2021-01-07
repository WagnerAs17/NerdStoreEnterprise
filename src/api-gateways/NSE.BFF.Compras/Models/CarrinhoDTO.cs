using System.Collections.Generic;

namespace NSE.BFF.Compras.Models
{
    public class CarrinhoDTO
    {
        public CarrinhoDTO()
        {
            Itens = new List<ItemCarrinhoDTO>();
        }

        public decimal ValorTotal { get; set; }
        public decimal ValorDesconto { get; set; }
        public List<ItemCarrinhoDTO> Itens { get; set; }
    }
}
