using Microsoft.EntityFrameworkCore;
using NSE.Cliente.API.Data.Repositories.Interfaces;
using NSE.Cliente.API.Models;
using NSE.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSE.Cliente.API.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ClienteContext _context;

        public ClienteRepository(ClienteContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Models.Cliente> ObterPorCpf(string cpf)
        {
            return await _context.Clientes.FirstOrDefaultAsync(x => x.Cpf.Numero == cpf);
        }

        public async Task<IEnumerable<Models.Cliente>> ObterTodos()
        {
            return await _context.Clientes.AsNoTracking().ToListAsync();
        }
        public async Task Adicionar(Models.Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
        }

        public async Task<Endereco> ObterEnderecoPorId(Guid clienteId)
        {
            return await _context.Enderecos.FirstOrDefaultAsync(x => x.ClienteId == clienteId);
        }

        public async Task AdicionarEndereco(Endereco endereco)
        {
            await _context.Enderecos.AddAsync(endereco);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
