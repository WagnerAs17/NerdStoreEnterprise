using FluentValidation.Results;
using NSE.Carrinho.API.Models;
using System;
using System.Threading.Tasks;

namespace NSE.Carrinho.API.Business
{
    public interface ICarrinhoBusiness
    {
        Task<CarrinhoCliente> ObterCarrinhoCliente(Guid userId);
        Task<ValidationResult> AdicionarCarrinhoCliente(Guid userId, CarrinhoItem item);
        Task<ValidationResult> UpdateCarrinho(Guid userId, Guid produtoId, CarrinhoItem item);
        Task<ValidationResult> DeleteCarrinho(Guid userId, Guid produtoId);
        Task<ValidationResult> UpdateCarrinhoCliente(Voucher voucher, CarrinhoCliente carrinho);
        
    }
}
