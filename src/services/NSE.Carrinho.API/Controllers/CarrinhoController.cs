using Microsoft.AspNetCore.Authorization;
using NSE.WebApi.Core.Controllers;

namespace NSE.Carrinho.API.Controllers
{
    [Authorize]
    public class CarrinhoController : MainController
    {
    }
}
