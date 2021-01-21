using NSE.Pagamento.API.Models;
using System.Threading.Tasks;

namespace NSE.Pagamento.API.Facade
{
    public interface IPagamentoFacade
    {
        Task<Transacao> AutorizarPagamento(Models.Pagamento pagamento);
    }
}
