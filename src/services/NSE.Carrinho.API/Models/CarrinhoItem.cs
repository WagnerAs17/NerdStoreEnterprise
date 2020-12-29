using FluentValidation;
using System;

namespace NSE.Carrinho.API.Models
{
    public class CarrinhoItem
    {
        public CarrinhoItem()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Guid ProdutoId { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
        public string Imagem { get; set; }
        public Guid CarrinhoId { get; set; }
        public CarrinhoCliente CarrinhoCliente { get; set; }

        internal void AssociarCarrinho(Guid carrinhoId)
        {
            CarrinhoId = carrinhoId;
        }

        internal decimal CalcularValor()
        {
            return Quantidade * Valor;
        }

        internal void AdicionarUnidades(int unidades)
        {
            Quantidade += unidades;
        }

        internal bool EhValido()
        {
            return new ItemPedidoValidation().Validate(this).IsValid;
        }
    }

    public class ItemPedidoValidation : AbstractValidator<CarrinhoItem>
    {
        public ItemPedidoValidation()
        {
            RuleFor(x => x.ProdutoId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id produto inválido.");

            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("O nome do produto não foi informado.");

            RuleFor(x => x.Quantidade)
                .GreaterThan(0)
                .WithMessage("A quantidade miníma de um item é 1.");

            RuleFor(x => x.Quantidade)
                .LessThan(5)
                .WithMessage("A quantidade máxima de um item é 5.");

            RuleFor(x => x.Valor)
                .GreaterThan(0)
                .WithMessage("O valor do item precisa ser maior do que 0.");
        }
    }
}
