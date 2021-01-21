using NSE.Core.Data;
using NSE.Pagamento.API.Models;
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

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
