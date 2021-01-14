using FluentValidation;
using NSE.Core.Messages;
using NSE.Pedido.API.Application.DTO;
using System;
using System.Collections.Generic;

namespace NSE.Pedido.API.Application.Commands
{
    public class AdicionarPedidoCommand : Command
    {
        //Pedido
        public Guid ClienteId { get; set; }
        public decimal ValorTotal { get; set; }
        public List<PedidoItemDTO> PedidoItems { get; set; }

        //Voucher
        public string VoucherCodigo { get; set; }
        public bool VoucherUtilizado { get; set; }
        public decimal Desconto { get; set; }

        //Endereco
        public EnderecoDTO Endereco { get; set; }

        //Cartao
        public string NumeroCartao { get; set; }
        public string NomeCartao { get; set; }
        public string ExpiracaoCartao { get; set; }
        public string CvvCartao { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new AdicionarPedidoValidation().Validate(this);

            return ValidationResult.IsValid;
        }
    }

    public class AdicionarPedidoValidation : AbstractValidator<AdicionarPedidoCommand>
    {
        public AdicionarPedidoValidation()
        {
            RuleFor(x => x.ClienteId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id do cliente inválido.");

            RuleFor(x => x.PedidoItems.Count)
                .GreaterThan(0)
                .WithMessage("O pedido precisa ter no mínimo 1 item.");

            RuleFor(x => x.ValorTotal)
                .GreaterThan(0)
                .WithMessage("O valor do pedido é invalido.");

            RuleFor(x => x.NumeroCartao)
                .CreditCard()
                .WithMessage("Número de cartão inválido.");

            RuleFor(x => x.NomeCartao)
                .NotNull()
                .WithMessage("Nome do portador do cartão é obrigatório.");

            RuleFor(x => x.CvvCartao.Length)
                .GreaterThan(2)
                .LessThan(5)
                .WithMessage("O CVV do cartão precisa ter 3 ou 4 números");

            RuleFor(x => x.ExpiracaoCartao)
                .NotNull()
                .WithMessage("Data expiração do cartão é obrigatório.");
        }
    }
}
