using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Carrinho.API.Business;
using NSE.Carrinho.API.Models;
using NSE.WebApi.Core.Controllers;
using NSE.WebApi.Core.Usuario;
using System;
using System.Threading.Tasks;

namespace NSE.Carrinho.API.Controllers
{
    [Authorize]
    [Route("api/carrinho")]
    public class CarrinhoController : MainController
    {
        private readonly IAspNetUser _aspNetUser;
        private readonly ICarrinhoBusiness _carrinhoBusiness;

        public CarrinhoController
        ( 
            IAspNetUser aspNetUser,
            ICarrinhoBusiness carrinhoBusiness
        )
        {
            _aspNetUser = aspNetUser;
            _carrinhoBusiness = carrinhoBusiness;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<CarrinhoCliente> ObterCarrinho()
        {
            return await _carrinhoBusiness.ObterCarrinhoCliente(_aspNetUser.ObterUserId());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AdicionarItemCarrinho(CarrinhoItem item)
        {
            var validation = await _carrinhoBusiness.AdicionarCarrinhoCliente(_aspNetUser.ObterUserId(), item);

            return CustomResponse(validation);
        }

        [HttpPut("{produtoId}")]
        public async Task<IActionResult> AtualizarItemCarrinho(Guid produtoId, CarrinhoItem item)
        {
            var validation = await _carrinhoBusiness.UpdateCarrinho(_aspNetUser.ObterUserId(), produtoId, item);

            return CustomResponse(validation);
        }

        [HttpDelete("{produtoId}")]
        public async Task<IActionResult> ExcluirItemCarrinho(Guid produtoId)
        {
            var validation = await _carrinhoBusiness.DeleteCarrinho(_aspNetUser.ObterUserId(), produtoId);

            return CustomResponse(validation);
        }
    }
}
