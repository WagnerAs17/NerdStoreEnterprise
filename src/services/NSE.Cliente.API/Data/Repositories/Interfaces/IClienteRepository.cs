using NSE.Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSE.Cliente.API.Data.Repositories.Interfaces
{
    public interface IClienteRepository : IRepository<Models.Cliente>
    {
        Task Adicionar(Models.Cliente cliente);
        Task<IEnumerable<Models.Cliente>> ObterTodos();
        Task<Models.Cliente> ObterPorCpf(string cpf);
    }
}
