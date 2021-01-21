using NSE.Core.Data;
using System.Threading.Tasks;

namespace NSE.Pagamento.API.Models
{
    public interface IPagamentoRepository : IRepository<Pagamento>
    {
        Task AdicionarPagamento(Pagamento pagamento);
    }
}
