using NSE.Carrinho.API.Models;
using System;
using System.Threading.Tasks;

namespace NSE.Carrinho.API.Data.Repository
{
    public interface ICarrinhoRepository
    {
        Task<CarrinhoCliente> ObterCarrinhoCliente(Guid userId);
        Task AdicionarCarrinhoCliente(CarrinhoCliente carrinho);
        Task AdicionarCarrinhoItem(CarrinhoItem item);
        void UpdateCarrinho(CarrinhoCliente carrinhos);
        void UpdateCarrinhoItem(CarrinhoItem item);

        Task<int> SaveChangesAsync();
    }
}
