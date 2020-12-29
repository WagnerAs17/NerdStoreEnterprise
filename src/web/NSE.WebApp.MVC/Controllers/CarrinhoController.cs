using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Services;
using System;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Controllers
{
    public class CarrinhoController : MainController
    {
        private readonly ICarrinhoService _carrinhoService;
        private readonly ICatalogoService _catalogoService;

        public CarrinhoController
        (
            ICarrinhoService carrinhoService,
            ICatalogoService catalogoService
        )
        {
            _carrinhoService = carrinhoService;
            _catalogoService = catalogoService;
        }

        [Route("carrinho")]
        public async Task<IActionResult> Index()
        {
            return View(await _carrinhoService.ObterCarrinho());
        }

        [HttpPost]
        [Route("carrinho/adicionar-item")]
        public async Task<IActionResult> AdicionarItemCarrinho(ItemProdutoViewModel itemProdutoViewModel)
        {
            var produto = await _catalogoService.ObterPorId(itemProdutoViewModel.ProdutoId);

            if(produto == null)
            {
                return ValidarProduto();
            }

            ValidarItemCarrinho(produto, itemProdutoViewModel.Quantidade);

            if (!OperacaoValida()) return View("Index", _carrinhoService.ObterCarrinho());

            itemProdutoViewModel.Nome = produto.Nome;
            itemProdutoViewModel.Valor = produto.Valor;
            itemProdutoViewModel.Imagem = produto.Imagem;

            var response = await _carrinhoService.AdicionarItemCarrinho(itemProdutoViewModel);

            if(ResponsePossuiErros(response)) return View("Index", _carrinhoService.ObterCarrinho());
             
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("carrinho/atualizar-item")]
        public async Task<IActionResult> AtualizarItemCarrinho(Guid produtoId, int quantidade)
        {
            var produto = await _catalogoService.ObterPorId(produtoId);

            if (produto == null)
            {
                return ValidarProduto();
            } 

            ValidarItemCarrinho(produto, quantidade);

            if(!OperacaoValida()) return View("Index", _carrinhoService.ObterCarrinho());

            var itemProduto = new ItemProdutoViewModel { ProdutoId = produtoId, Quantidade = quantidade };

            var response = await _carrinhoService.AtualizarItemCarrinho(itemProduto);

            if (ResponsePossuiErros(response)) return View("Index", _carrinhoService.ObterCarrinho());
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("carrinho/remover-item")]
        public async Task<IActionResult> RemoverItemCarrinho(Guid produtoId)
        {
            var produto = await _catalogoService.ObterPorId(produtoId);

            if (produto == null)
            {
                return ValidarProduto();
            }

            var response = await _carrinhoService.RemoverItemCarrinho(produtoId);

            if (ResponsePossuiErros(response)) return View("Index", _carrinhoService.ObterCarrinho());

            return RedirectToAction("Index");
        }

        private void ValidarItemCarrinho(ProdutoViewModel produto, int quantidade)
        {
            if (quantidade < 1)
                AdicionarErroValidacao($"Escolha ao menos uma unidade do produto {produto.Nome}");

            if (quantidade > produto.QuantidadeEstoque) 
                AdicionarErroValidacao($"O produto {produto.Nome} possui {produto.QuantidadeEstoque} unidades em estoque, você selecionou {quantidade}.");
        }

        private IActionResult ValidarProduto()
        {
            AdicionarErroValidacao("Produto não existe.");

            return View("Index", _carrinhoService.ObterCarrinho());
        }
    }
}
