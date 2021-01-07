using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.WebApi.Core.Controllers;

namespace NSE.Pedido.API.Controllers
{
    [Authorize]
    public class PedidoController : MainController
    {
        [HttpGet]
        public IActionResult ObterPedidos()
        {
            return CustomResponse();
        }

        [HttpPost]
        public IActionResult CadastrarPedido()
        {
            return CustomResponse();
        }
    }
}
