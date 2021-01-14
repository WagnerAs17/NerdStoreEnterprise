using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using NSE.Pedidos.Domain.Pedidos;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.Pedidos.Infra.Data.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly PedidosContext _context;

        public PedidoRepository(PedidosContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public DbConnection ObterConexao()
        {
            return _context.Database.GetDbConnection();
        }
        public async Task<Pedido> ObterPorId(Guid id)
        {
            return await _context.Pedidos.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PedidoItem> ObterItemPorId(Guid id)
        {
            return await _context.PedidoItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PedidoItem> ObterItemPorPedido(Guid pedidoId, Guid produtoId)
        {
            return await _context.PedidoItems.FirstOrDefaultAsync(
                x => x.PedidoId == pedidoId && x.ProdudoId == produtoId);
        }

        public async Task<IEnumerable<Pedido>> ObterListaPorClienteId(Guid clienteId)
        {
            return await _context.Pedidos
                .Include(x => x.PedidoItems)
                .AsNoTracking()
                .Where(x => x.ClienteId == clienteId)
                .ToArrayAsync();
        }

        public async Task Adicionar(Pedido pedido)
        {
            await _context.AddAsync(pedido);
        }

        public void Update(Pedido pedido)
        {
            _context.Update(pedido);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
