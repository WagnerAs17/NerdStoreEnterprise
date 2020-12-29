using Microsoft.EntityFrameworkCore;
using NSE.Carrinho.API.Models;
using System;
using System.Threading.Tasks;

namespace NSE.Carrinho.API.Data.Repository
{
    public class CarrinhoRepository : ICarrinhoRepository
    {
        private readonly CarrinhoContext _context;
        public CarrinhoRepository(CarrinhoContext context)
        {
            _context = context;
        }

        public async Task<CarrinhoCliente> ObterCarrinhoCliente(Guid userId)
        {
            return await _context.CarrinhoCliente
                .Include(x => x.Itens)
                .FirstOrDefaultAsync(x => x.ClienteId == userId);
        }
        
        public async Task AdicionarCarrinhoCliente(CarrinhoCliente carrinho)
        {
            await _context.CarrinhoCliente.AddAsync(carrinho);
        }

        public async Task AdicionarCarrinhoItem(CarrinhoItem item)
        {
            await _context.CarrinhoItems.AddAsync(item);
        }

        public void UpdateCarrinho(CarrinhoCliente carrinho)
        {
            _context.CarrinhoCliente.Update(carrinho);
        }

        public void UpdateCarrinhoItem(CarrinhoItem item)
        {
            _context.CarrinhoItems.Update(item);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
