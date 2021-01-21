using FluentValidation;
using NSE.Core.Messages;
using System;

namespace NSE.Cliente.API.Application.Commands
{
    public class AdicionarEnderecoCommand : Command
    {
        public Guid ClienteId { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }

        public AdicionarEnderecoCommand()
        {

        }
        public AdicionarEnderecoCommand
        (
            Guid clienteId,
            string logradouro,
            string numero,
            string complemento,
            string bairro,
            string cep,
            string cidade,
            string estado
        )
        {
            AggregateId = clienteId;
            ClienteId = clienteId;
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cep = cep;
            Cidade = cidade;
            Estado = estado;
        }

        public override bool EhValido()
        {
            ValidationResult = new AdicionarEnderecoValidation().Validate(this);

            return ValidationResult.IsValid;
        }
    }

    public class AdicionarEnderecoValidation : AbstractValidator<AdicionarEnderecoCommand>
    {
        public AdicionarEnderecoValidation()
        {
            RuleFor(x => x.Logradouro)
                .NotEmpty()
                .WithMessage("Informe o logradouro.");

            RuleFor(x => x.Numero)
                .NotEmpty()
                .WithMessage("Informe o número.");

            RuleFor(x => x.Cep)
                .NotEmpty()
                .WithMessage("Informe o cep");

            RuleFor(x => x.Bairro)
                .NotEmpty()
                .WithMessage("Informe o bairro.");

            RuleFor(x => x.Cidade)
                .NotEmpty()
                .WithMessage("Informe a cidade.");

            RuleFor(x => x.Estado)
                .NotEmpty()
                .WithMessage("Informe o estado");

        }
    }
}
