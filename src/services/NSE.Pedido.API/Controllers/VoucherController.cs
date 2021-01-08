using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSE.Pedido.API.Application.DTO;
using NSE.Pedido.API.Application.Queries;
using NSE.WebApi.Core.Controllers;
using System.Threading.Tasks;

namespace NSE.Pedido.API.Controllers
{
    [Authorize]
    public class VoucherController : MainController
    {
        private readonly IVoucherQueries _voucherQueries;

        public VoucherController(IVoucherQueries voucherQueries)
        {
            _voucherQueries = voucherQueries;
        }

        [HttpGet("voucher/{codigo}")]
        [ProducesResponseType(typeof(VoucherDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterVoucherPorCodigo(string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) return NotFound();

            var voucher = await _voucherQueries.ObterVoucherPorCodido(codigo);

            return voucher == null ? NotFound() : CustomResponse(voucher);
        }
    }
}
