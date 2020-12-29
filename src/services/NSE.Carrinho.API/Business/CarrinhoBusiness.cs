using NSE.Carrinho.API.Data.Repository;
using NSE.Carrinho.API.Models;
using System;
using System.Threading.Tasks;

namespace NSE.Carrinho.API.Business
{
    public class CarrinhoBusiness : ICarrinhoBusiness
    {
        private readonly ICarrinhoRepository _carrinhoRepository;

        public CarrinhoBusiness(ICarrinhoRepository carrinhoRepository)
        {
            _carrinhoRepository = carrinhoRepository;
        }

        public async Task<CarrinhoCliente> ObterCarrinhoCliente(Guid userId)
        {
            return await _carrinhoRepository.ObterCarrinhoCliente(userId);
        }

        public async Task<int> AdicionarCarrinhoCliente(Guid userId, CarrinhoItem item)
        {
            var carrinho = await ObterCarrinhoCliente(userId);

            if (carrinho == null)
                await _carrinhoRepository.AdicionarCarrinhoCliente(ManipularNovoCarrinho(userId, item));
            else
                await ManipularCarrinhoExistente(carrinho, item);

            return await _carrinhoRepository.SaveChangesAsync();
        }

        private CarrinhoCliente ManipularNovoCarrinho(Guid userId, CarrinhoItem item)
        {
            var carrinho = new CarrinhoCliente(userId);

            carrinho.AdicionarItem(item);

            return carrinho;
        }

        private async Task ManipularCarrinhoExistente(CarrinhoCliente carrinho, CarrinhoItem item)
        {
            var produtoItemExistente = carrinho.CarrinhoItemExistente(item);

            carrinho.AdicionarItem(item);

            if (produtoItemExistente)
                _carrinhoRepository.UpdateCarrinhoItem(carrinho.ObterPorProdutoId(item.ProdutoId));
            else
                await _carrinhoRepository.AdicionarCarrinhoItem(item);

            await _carrinhoRepository.AdicionarCarrinhoCliente(carrinho);
        }
    }
}
