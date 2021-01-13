using FluentValidation.Results;
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

        public async Task<ValidationResult> AdicionarCarrinhoCliente(Guid userId, CarrinhoItem item)
        {
            var carrinho = await ObterCarrinhoCliente(userId);

            if (carrinho == null)
            {
                carrinho = new CarrinhoCliente(userId);
                await _carrinhoRepository.AdicionarCarrinhoCliente(ManipularNovoCarrinho(userId, item, carrinho));
            }
            else
                await ManipularCarrinhoExistente(carrinho, item);

            if (!carrinho.EhValido()) return carrinho.ValidationResult;

            await PersistirDados(carrinho.ValidationResult);

            return carrinho.ValidationResult;
        }

        public async Task<ValidationResult> UpdateCarrinhoCliente(Voucher voucher, CarrinhoCliente carrinho)
        {
            var validation = new ValidationResult();

            carrinho.AplicarVoucher(voucher);

            _carrinhoRepository.UpdateCarrinho(carrinho);

            await PersistirDados(validation);

            return validation;
        }

        public async Task<ValidationResult> UpdateCarrinho(Guid userId, Guid produtoId, CarrinhoItem item)
        {
            var validation = new ValidationResult();

            var carrinho = await ObterCarrinhoCliente(userId);

            var itemCarrinho = await ObterItemCarrinhoValidado(produtoId, validation, carrinho, item);

            if (itemCarrinho != null)
            {
                carrinho.AtualizarUnidades(itemCarrinho, item.Quantidade);
                
                if (!carrinho.EhValido()) return carrinho.ValidationResult;

                _carrinhoRepository.UpdateCarrinho(carrinho);
                _carrinhoRepository.UpdateCarrinhoItem(itemCarrinho);
                
                await PersistirDados(validation);
            }

            return validation;
        }

        public async Task<ValidationResult> DeleteCarrinho(Guid userId, Guid produtoId)
        {
            var validation = new ValidationResult();

            var carrinho = await ObterCarrinhoCliente(userId);

            var itemCarrinho = await ObterItemCarrinhoValidado(produtoId, validation, carrinho);

            if(itemCarrinho != null)
            {
                carrinho.RemoverItem(itemCarrinho);
                
                if (!carrinho.EhValido()) return carrinho.ValidationResult;

                _carrinhoRepository.DeleteCarrinhoItem(itemCarrinho);

                _carrinhoRepository.UpdateCarrinho(carrinho);

                await PersistirDados(validation);
            }

            return validation;
        }

        private async Task PersistirDados(ValidationResult validation)
        {
            if (await _carrinhoRepository.SaveChangesAsync() <= 0)
                validation.Errors.Add(new ValidationFailure(string.Empty, "Erro ao persistir os dados no banco."));
        }

        private async Task<CarrinhoItem> ObterItemCarrinhoValidado
        (Guid produtoId, ValidationResult validation, CarrinhoCliente carrinho, CarrinhoItem item = null)
        {
            if (item != null && produtoId != item.ProdutoId)
            {
                validation.Errors.Add(new ValidationFailure(string.Empty, "Item não confere."));
                return null;
            }

            if (carrinho == null)
            {
                validation.Errors.Add(new ValidationFailure(string.Empty, "Carrinho não encontrado"));

                return null;
            }

            var itemCarrinho = await _carrinhoRepository.ObterCarrinhoItem(carrinho.Id, produtoId);

            if (itemCarrinho == null || !carrinho.CarrinhoItemExistente(itemCarrinho))
            {
                validation.Errors.Add(new ValidationFailure(string.Empty, "O item não está no carrinho"));

                return null;
            }

            return itemCarrinho;
        }
            
        private CarrinhoCliente ManipularNovoCarrinho(Guid userId, CarrinhoItem item, CarrinhoCliente carrinho)
        {
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

            _carrinhoRepository.UpdateCarrinho(carrinho);
        }
    }
}
