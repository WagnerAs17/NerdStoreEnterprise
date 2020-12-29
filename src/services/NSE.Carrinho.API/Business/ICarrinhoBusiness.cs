using NSE.Carrinho.API.Models;
using System;
using System.Threading.Tasks;

namespace NSE.Carrinho.API.Business
{
    public interface ICarrinhoBusiness
    {
        Task<CarrinhoCliente> ObterCarrinhoCliente(Guid userId);
        Task<int> AdicionarCarrinhoCliente(Guid userId, CarrinhoItem item);
    }
}
