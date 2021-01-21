using FluentValidation.Results;
using MediatR;
using NSE.Core.Messages;
using NSE.Core.Messages.Integrations;
using NSE.MessageBus;
using NSE.Pedido.API.Application.DTO;
using NSE.Pedido.API.Application.Events;
using NSE.Pedidos.Domain;
using NSE.Pedidos.Domain.Pedidos;
using NSE.Pedidos.Domain.Vouchers.Specs;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NSE.Pedido.API.Application.Commands
{
    public class PedidoCommandHandler : CommandHandler,
        IRequestHandler<AdicionarPedidoCommand, ValidationResult>
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IMessageBus _bus;

        public PedidoCommandHandler
        (
            IVoucherRepository voucherRepository,
            IPedidoRepository pedidoRepository,
            IMessageBus bus
        )
        {
            _voucherRepository = voucherRepository;
            _pedidoRepository = pedidoRepository;
            _bus = bus;
        }

        public async Task<ValidationResult> Handle(AdicionarPedidoCommand message, CancellationToken cancellationToken)
        {
            //Validacao do comando
            if (!message.EhValido()) return message.ValidationResult;

            //Mapear pedido
            var pedido = MapearPedido(message);

            //Aplicar voucher se houver
            if (!await AplicarVoucher(message, pedido)) return ValidationResult;

            //Validar pedido
            if (!ValidarPedido(pedido)) return ValidationResult;

            //Processar pagamento
            if (!await ProcessarPagamento(pedido, message)) return ValidationResult;

            //Se pagamento ok
            pedido.AutorizarPedido();

            //Adicionar Evento
            pedido.AdicionarEvento(new PedidoRealizadoEvent(pedido.Id, pedido.ClienteId));

            //Adicionar pedido no repositorio
            await _pedidoRepository.Adicionar(pedido);

            //Persistir dados do pedido e voucher
            return await PersistirDdados(_pedidoRepository.UnitOfWork);
        }

        private Pedidos.Domain.Pedidos.Pedido MapearPedido(AdicionarPedidoCommand message)
        {
            var endereco = new Pedidos.Domain.Pedidos.Endereco
            {
                Bairro = message.Endereco.Bairro,
                Cep = message.Endereco.Cep,
                Cidade = message.Endereco.Cidade,
                Complemento = message.Endereco.Complemento,
                Estado = message.Endereco.Estado,
                Logradouro = message.Endereco.Logradouro,
                Numero = message.Endereco.Numero
            };

            var pedido = new Pedidos.Domain.Pedidos.Pedido
                (
                    message.ClienteId,
                    message.ValorTotal,
                    message.PedidoItems.Select(PedidoItemDTO.ParaPedidoItem).ToList(),
                    message.VoucherUtilizado,
                    message.Desconto
                );

            pedido.AtribuirEndereco(endereco);

            return pedido;
        }

        private async Task<bool> AplicarVoucher(AdicionarPedidoCommand message, Pedidos.Domain.Pedidos.Pedido pedido)
        {
            if (!message.VoucherUtilizado) return true;

            var voucher = await _voucherRepository.ObterVoucherPorCodido(message.VoucherCodigo);
            if(voucher == null)
            {
                AdicionarErro("O voucher informado não existe!");
                return false;
            }

            var voucherValidation = new VoucherValidation().Validate(voucher);
            if (!voucherValidation.IsValid)
            {
                voucherValidation.Errors.ToList().ForEach(m => AdicionarErro(m.ErrorMessage));
            }

            pedido.AtribuirVoucher(voucher);
            voucher.DebitarQuantidade();

            _voucherRepository.Atualizar(voucher);

            return true;
        }

        private bool ValidarPedido(Pedidos.Domain.Pedidos.Pedido pedido)
        {
            var pedidoValorOriginal = pedido.ValorTotal;
            var pedidoDesconto = pedido.Desconto;

            //pedido.CalcularValorTotalDesconto();

            if(pedido.ValorTotal != pedidoValorOriginal)
            {
                AdicionarErro("O valor do total do pedido não confere com o cálculo do pedido");
                return false;
            }

            if(pedido.Desconto != pedidoDesconto)
            {
                AdicionarErro("O valor total não confere com o cálculo do pedido");
                return false;
            }

            return true;
        }

        private async Task<bool> ProcessarPagamento(Pedidos.Domain.Pedidos.Pedido pedido, AdicionarPedidoCommand message)
        {
            var pedidoIniciado = new PedidoIniciadoIntegrationEvent
            {
                PedidoId = pedido.Id,
                ClienteId = pedido.ClienteId,
                Valor = pedido.ValorTotal,
                TipoPagamento = 1, //Realizar mudança em implementação futura,
                NomeCartao = message.NomeCartao,
                NumeroCartao = message.NumeroCartao,
                MesAnoVencimento = message.ExpiracaoCartao,
                CVV = message.CvvCartao
            };

            var result = await _bus
                .RequestAsync<PedidoIniciadoIntegrationEvent, ResponseMessage>(pedidoIniciado);

            if (result.ValidationResult.IsValid) return true;


            foreach(var erro in result.ValidationResult.Errors)
            {
                AdicionarErro(erro.ErrorMessage);
            }

            return false;
        }
    }
}
