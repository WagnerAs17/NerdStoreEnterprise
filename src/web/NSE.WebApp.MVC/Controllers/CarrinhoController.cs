using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Services;
using System;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Controllers
{
    public class CarrinhoController : MainController
    {
        private readonly IComprasBffService _comprasBffService;
        public CarrinhoController
        (
            IComprasBffService comprasBffService
        )
        {
            _comprasBffService = comprasBffService;
        }

        [Route("carrinho")]
        public async Task<IActionResult> Index()
        {
            return View(await _comprasBffService.ObterCarrinho());
        }

        [HttpPost]
        [Route("carrinho/adicionar-item")]
        public async Task<IActionResult> AdicionarItemCarrinho(ItemCarrinhoViewModel itemProdutoViewModel)
        {
            var response = await _comprasBffService.AdicionarItemCarrinho(itemProdutoViewModel);

            if (ResponsePossuiErros(response)) return View("Index", await _comprasBffService.ObterCarrinho());

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("carrinho/atualizar-item")]
        public async Task<IActionResult> AtualizarItemCarrinho(Guid produtoId, int quantidade)
        {
            var itemProduto = new ItemCarrinhoViewModel { ProdutoId = produtoId, Quantidade = quantidade };

            var response = await _comprasBffService.AtualizarItemCarrinho(produtoId, itemProduto);

            if (ResponsePossuiErros(response)) return View("Index", await _comprasBffService.ObterCarrinho());

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("carrinho/remover-item")]
        public async Task<IActionResult> RemoverItemCarrinho(Guid produtoId)
        {
            var response = await _comprasBffService.RemoverItemCarrinho(produtoId);

            if (ResponsePossuiErros(response)) return View("Index", await _comprasBffService.ObterCarrinho());

            return RedirectToAction("Index");
        }
    }
}
