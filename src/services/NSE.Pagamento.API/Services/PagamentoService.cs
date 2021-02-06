using FluentValidation.Results;
using NSE.Core.DomainObjects;
using NSE.Core.Messages.Integrations;
using NSE.Pagamento.API.Facade;
using NSE.Pagamento.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.Pagamento.API.Services
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoFacade _pagamentoFacade;
        private readonly IPagamentoRepository _pagamentoRepository;

        public PagamentoService
        (
            IPagamentoFacade pagamentoFacade, 
            IPagamentoRepository pagamentoRepository
        )
        {
            _pagamentoFacade = pagamentoFacade;
            _pagamentoRepository = pagamentoRepository;
        }
        public async Task<ResponseMessage> AutorizarPagamento(Models.Pagamento pagamento)
        {
            var transacao = await _pagamentoFacade.AutorizarPagamento(pagamento);

            var validationResult = new ValidationResult();

            if(transacao.Status != StatusTransacao.Autorizado)
            {
                validationResult.Errors.Add(
                    new ValidationFailure("Pagamento", "Pagamento recusado, entre em contato com a sua operadora do cartão."));
                
                return new ResponseMessage(validationResult);
            }

            pagamento.AdicionarTransacao(transacao);

            await _pagamentoRepository.AdicionarPagamento(pagamento);

            if(!await _pagamentoRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(
                    new ValidationFailure("Pagamento", "Houve um erro ao realizar o pagamento."));

                //TODO: Comunicar com o gateway de pagamento para realizar o estorno.
                //Caso tenha mais APIS escutando no bus(messageria) publicar a mensagem na fila.
                //Se simplesmente só quer realizar o retorno, implementar um método para fazer isso.

                return new ResponseMessage(validationResult);
            }

            return new ResponseMessage(validationResult);
        }

        public async Task<ResponseMessage> CancelarPagamento(Guid pedidoId)
        {
            var transacoes = await _pagamentoRepository.ObterTransacoesPorPedidoId(pedidoId);

            var transacaoAutorizada = transacoes?.FirstOrDefault(x => x.Status == StatusTransacao.Autorizado);

            var validation = new ValidationResult();

            if (transacaoAutorizada == null) throw new DomainException($"Transacao não encontrada para o pedido: {pedidoId}");

            var transacao = await _pagamentoFacade.CancelarPagamento(transacaoAutorizada);

            if(transacao.Status != StatusTransacao.Negado)
            {
                validation.Errors.Add(
                    new ValidationFailure("Pagamento", $"Não foi possível cancelar o pagamento do pedido: {pedidoId}"));

                return new ResponseMessage(validation);
            }

            transacao.PagamentoId = transacaoAutorizada.PagamentoId;

            await _pagamentoRepository.AdicionarTransacao(transacao);

            if (!await _pagamentoRepository.UnitOfWork.Commit())
            {
                validation.Errors.Add(
                    new ValidationFailure("Pagamento",
                    $"Não foi possível persistir o cancelamento do pagamento para o pedido {pedidoId}"));

                return new ResponseMessage(validation);
            }

            return new ResponseMessage(validation);
        }

        public async Task<ResponseMessage> CapturarPagamento(Guid pedidoId)
        {
            var transacoes = await _pagamentoRepository.ObterTransacoesPorPedidoId(pedidoId);

            var transacaoAutorizada = transacoes?.FirstOrDefault(x => x.Status == StatusTransacao.Autorizado);

            var validation = new ValidationResult();

            if (transacaoAutorizada == null) throw new DomainException($"Transacao não encontrada para o pedido: {pedidoId}");

            var transacao = await _pagamentoFacade.CapturarPagamento(transacaoAutorizada);

            if (transacao.Status != StatusTransacao.Pago)
            {
                validation.Errors.Add(
                    new ValidationFailure("Pagamento", $"Não foi possível capturar o pagamento do pedido: {pedidoId}"));

                return new ResponseMessage(validation);
            }

            transacao.PagamentoId = transacaoAutorizada.PagamentoId;

            await _pagamentoRepository.AdicionarTransacao(transacao);

            if (!await _pagamentoRepository.UnitOfWork.Commit())
            {
                validation.Errors.Add(
                    new ValidationFailure("Pagamento",
                    $"Não foi possível persistir a captura do pagamento para o pedido {pedidoId}"));

                return new ResponseMessage(validation);
            }

            return new ResponseMessage(validation);
        }
    }
}
