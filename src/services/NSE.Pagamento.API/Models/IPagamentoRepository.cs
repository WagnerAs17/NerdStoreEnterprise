using NSE.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSE.Pagamento.API.Models
{
    public interface IPagamentoRepository : IRepository<Pagamento>
    {
        Task AdicionarPagamento(Pagamento pagamento);
        Task<IEnumerable<Transacao>> ObterTransacoesPorPedidoId(Guid pedidoId);
        Task AdicionarTransacao(Transacao transacao);
    }
}
