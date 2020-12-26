using Microsoft.EntityFrameworkCore;
using NSE.Catalogo.API.Models;
using NSE.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSE.Catalogo.API.Data.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly CatologoContext context;

        public ProdutoRepository(CatologoContext context)
        {
            this.context = context;
        }

        public IUnitOfWork UnitOfWork => context;

        public async Task<Produto> ObterPorId(Guid id)
        {
            return await context.Produtos.FindAsync(id);
        }

        public async Task<IEnumerable<Produto>> ObterTodos()
        {
            return await context.Produtos.AsNoTracking().ToListAsync();
        }

        public async Task Adicionar(Produto produto)
        {
            await context.Produtos.AddAsync(produto);
        }

        public void Atualizar(Produto produto)
        {
            context.Produtos.Update(produto);
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
