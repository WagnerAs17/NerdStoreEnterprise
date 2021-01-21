using Microsoft.Extensions.Options;
using NSE.Pagamento.API.Facade;
using NSE.Pagamento.API.Models;
using NSE.Pagamentos.NerdsPag;
using System;
using System.Threading.Tasks;

namespace NSE.Pagamento.CardAntiCorruption
{
    public class PagamentoCartaoCreditoFacade : IPagamentoFacade
    {
        private readonly PagamentoConfig _pagamentoConfig;

        public PagamentoCartaoCreditoFacade(IOptions<PagamentoConfig> options)
        {
            _pagamentoConfig = options.Value;
        }

        public async Task<Transacao> AutorizarPagamento(API.Models.Pagamento pagamento)
        {
            var nerdsPag = new NerdsPagService(
                _pagamentoConfig.DefaultApiKey, _pagamentoConfig.DefaultEncryptionKey);

            var cardHashGen = new CardHash(nerdsPag)
            {
                CardCvv = pagamento.CartaoCredito.CVV,
                CardExpirationDate = pagamento.CartaoCredito.MesAnoVencimento,
                CardHolderName = pagamento.CartaoCredito.NomeCartao,
                CardNumber = pagamento.CartaoCredito.NumeroCartao
            };

            var cardHash = cardHashGen.Generate();

            var transacao = new Transaction(nerdsPag)
            {
                CardHash = cardHash,
                CardNumber = pagamento.CartaoCredito.NumeroCartao,
                CardHolderName = pagamento.CartaoCredito.NomeCartao,
                CardExpirationDate = pagamento.CartaoCredito.MesAnoVencimento,
                CardCvv = pagamento.CartaoCredito.CVV,
                PaymentMethod = PaymentMethod.CreditCard,
                Amount = pagamento.Valor
            };

            return ParaTransacao(await transacao.AuthorizeCardTransaction());
        }

        public static Transacao ParaTransacao(Transaction transaction)
        {
            return new Transacao
            {
                Id = Guid.NewGuid(),
                Status = (StatusTransacao)transaction.Status,
                ValorTotal = transaction.Amount,
                BandeiraCartao = transaction.CardBrand,
                CodigoAutorizacao = transaction.AuthorizationCode,
                CustoTransacao = transaction.Cost,
                DataTransacao = transaction.TransactionDate,
                NSU = transaction.Nsu,
                TID = transaction.Tid
            };
        }
    }
}
