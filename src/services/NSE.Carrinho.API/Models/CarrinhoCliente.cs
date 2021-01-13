using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NSE.Carrinho.API.Models
{
    public class CarrinhoCliente
    {

        public const int QuantidadeMaximaItem = 5;
        public CarrinhoCliente(){}
        
        public CarrinhoCliente(Guid clienteId)
        {
            Id = Guid.NewGuid();
            ClienteId = clienteId;
        }
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public decimal ValorTotal { get; set; }
        public List<CarrinhoItem> Itens { get; set; } = new List<CarrinhoItem>();
        public ValidationResult ValidationResult { get; set; }
        public bool VoucherUtilizado { get; set; }
        public decimal Desconto { get; set; }
        public Voucher Voucher { get; set; }

        public void AplicarVoucher(Voucher voucher)
        {
            Voucher = voucher;
            VoucherUtilizado = true;
            CalcularValorCarrinho();
        }

        private void CalcularValorTotalDesconto()
        {
            if (!VoucherUtilizado) return;

            decimal desconto = 0;
            var valor = ValorTotal;
            if(Voucher.TipoDesconto == TipoDescontoVoucher.Porcentagem)
            {
                if (Voucher.Percentual.HasValue)
                {
                    desconto = (valor * Voucher.Percentual.Value) / 100;
                    valor -= desconto;
                }
            }
            else
            {
                if (Voucher.ValorDesconto.HasValue)
                {
                    desconto = Voucher.ValorDesconto.Value;
                    valor -= desconto;
                }
            }

            ValorTotal = valor < 0 ? 0 : valor;
            Desconto = desconto;
        }

        internal void CalcularValorCarrinho()
        {
            ValorTotal = Itens.Sum(item => item.CalcularValor());
            CalcularValorTotalDesconto();
        }

        internal bool CarrinhoItemExistente(CarrinhoItem item)
        {
            return Itens.Any(i => i.ProdutoId == item.ProdutoId);
        }

        internal CarrinhoItem ObterPorProdutoId(Guid produtoId) 
        {
            return Itens.FirstOrDefault(item => item.ProdutoId == produtoId);
        }

        internal void AdicionarItem(CarrinhoItem item)
        {
            item.AssociarCarrinho(Id);

            if (CarrinhoItemExistente(item))
            {
                var itemExistente = ObterPorProdutoId(item.ProdutoId);
                item.AdicionarUnidades(itemExistente.Quantidade);

                item = itemExistente;

                Itens.Remove(itemExistente);
            }

            Itens.Add(item);
                
            CalcularValorCarrinho();
        }

        internal void AtualizarItem(CarrinhoItem item)
        {
            item.AssociarCarrinho(Id);

            var itemExistente = ObterPorProdutoId(item.ProdutoId);

            Itens.Remove(itemExistente);
            Itens.Add(item);

            CalcularValorCarrinho();
        }

        internal void AtualizarUnidades(CarrinhoItem item, int unidades)
        {
            item.AtualizarUnidades(unidades);
            AtualizarItem(item);
        }

        internal void RemoverItem(CarrinhoItem item)
        {
            Itens.Remove(ObterPorProdutoId(item.ProdutoId));

            CalcularValorCarrinho();
        }

        internal bool EhValido()
        {
            var errors = Itens.SelectMany(x => new ItemCarrinhoValidation().Validate(x).Errors).ToList();
            errors.AddRange(new CarrinhoClienteValidation().Validate(this).Errors);

            ValidationResult = new ValidationResult(errors);

            return ValidationResult.IsValid;
        }
    }

    public class CarrinhoClienteValidation : AbstractValidator<CarrinhoCliente>
    {
        public CarrinhoClienteValidation()
        {
            RuleFor(x => x.ClienteId)
                .NotEqual(Guid.Empty)
                .WithMessage("Cliente não conhecido");

            RuleFor(x => x.Itens.Count)
                .GreaterThan(0)
                .WithMessage("O carrinho não possui itens");

            RuleFor(x => x.ValorTotal)
                .GreaterThan(0)
                .WithMessage("O valor total do carrinho precisa ser maior que 0");
        }
    }

}
