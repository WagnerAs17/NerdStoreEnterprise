using NSE.Core.Messages.Integrations;
using System.Threading.Tasks;

namespace NSE.Pagamento.API.Services
{
    public interface IPagamentoService
    {
        Task<ResponseMessage> AutorizarPagamento(Models.Pagamento pagamento);
    }
}
