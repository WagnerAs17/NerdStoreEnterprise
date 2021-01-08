using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using NSE.Pedidos.Domain;
using System.Threading.Tasks;

namespace NSE.Pedidos.Infra.Data.Repository
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly PedidosContext _context;

        public VoucherRepository(PedidosContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Voucher> ObterVoucherPorCodido(string codigo)
        {
            return await _context.Vouchers.FirstOrDefaultAsync(x => x.Codigo == codigo);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
