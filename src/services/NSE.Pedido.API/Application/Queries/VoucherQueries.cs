using NSE.Pedido.API.Application.DTO;
using NSE.Pedidos.Domain;
using System.Threading.Tasks;

namespace NSE.Pedido.API.Application.Queries
{
    public interface IVoucherQueries
    {
        Task<VoucherDTO> ObterVoucherPorCodido(string codigo);
    }
    public class VoucherQueries : IVoucherQueries
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherQueries(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<VoucherDTO> ObterVoucherPorCodido(string codigo)
        {
            var voucher = await _voucherRepository.ObterVoucherPorCodido(codigo);

            if (voucher == null) return null;

            if (!voucher.ValidoParaUtilizazao()) return null;

            return new VoucherDTO
            {
                Codigo = voucher.Codigo,
                Percentual = voucher.Percentual,
                ValorDesconto = voucher.ValorDesconto,
                TipoDesconto = (int)voucher.TipoDesconto
            };
        }
    }
}
