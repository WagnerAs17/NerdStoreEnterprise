using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using NSE.Pagamento.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.Pagamento.API.Data.Repository
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly PagamentoContext _context;

        public PagamentoRepository(PagamentoContext context)
        {
            _context = context;
        }
        public IUnitOfWork UnitOfWork => _context;

        public async Task AdicionarPagamento(Models.Pagamento pagamento)
        {
            await _context.Pagamentos.AddAsync(pagamento);
        }

        public async Task<IEnumerable<Transacao>> ObterTransacoesPorPedidoId(Guid pedidoId)
        {
            return await _context.Transacoes.AsNoTracking()
                .Where(x => x.Pagamento.PedidoId == pedidoId).ToListAsync();
        }

        public async Task AdicionarTransacao(Transacao transacao)
        {
            await _context.Transacoes.AddAsync(transacao);
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
