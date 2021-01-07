using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.BFF.Compras.Models;
using NSE.BFF.Compras.Services;
using NSE.WebApi.Core.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.BFF.Compras.Controllers
{
    [Authorize]
    public class CarrinhoController : MainController    
    {
        private readonly ICatalogoService _catalogoService;
        private readonly ICarrinhoService _carrinhoService;
        public CarrinhoController
        (
            ICatalogoService catalogoService,
            ICarrinhoService carrinhoService
        )
        {
            _catalogoService = catalogoService;
            _carrinhoService = carrinhoService;
        }

        [HttpGet]
        [Route("compras/carrinho")]
        public async Task<IActionResult> Index()
        {
            return CustomResponse(await _carrinhoService.ObterCarrinho());
        }

        [HttpGet]
        [Route("compras/carrinho-quantidade")]
        public async Task<int> ObterQuantidadeCarrinho()
        {
            var quantidade = await _carrinhoService.ObterCarrinho();

            return quantidade?.Itens.Sum(x => x.Quantidade) ?? 0;
        }

        [HttpPost]
        [Route("compras/carrinho/items")]
        public async Task<IActionResult> AdicionarItemCarrinho(ItemCarrinhoDTO itemProduto)
        {
            var produto = await _catalogoService.ObterPorId(itemProduto.ProdutoId);

            await ValidarItemCarrinho(produto, itemProduto.Quantidade);

            if (!OperacaoValida()) return CustomResponse();

            itemProduto.Nome = produto.Nome;
            itemProduto.Imagem = produto.Imagem;
            itemProduto.Valor = produto.Valor;

            var resposta = await _carrinhoService.AdicionarItemCarrinho(itemProduto);

            return CustomResponse(resposta);
        }

        [HttpPut]
        [Route("compras/carrinho/items/{produtoId}")]
        public async Task<IActionResult> AtualizarItemCarrinho(Guid produtoId, ItemCarrinhoDTO itemProduto)
        {
            var produto = await _catalogoService.ObterPorId(produtoId);

            await ValidarItemCarrinho(produto, itemProduto.Quantidade);

            if (!OperacaoValida()) return CustomResponse();

            var response = await _carrinhoService.AtualizarItemCarrinho(produtoId, itemProduto);

            return CustomResponse(response);
        }

        [HttpDelete]
        [Route("compras/carrinho/items/{produtoId}")]
        public async Task<IActionResult> RemoverItemCarrinho(Guid produtoId)
        {
            var produto = await _catalogoService.ObterPorId(produtoId);

            if(produto == null)
            {
                AdicionarErroProcessamento("Produto não existe.");

                return CustomResponse();
            }

            var response = await _carrinhoService.RemoverItemCarrinho(produtoId);

            return CustomResponse(response);
        }

        private async Task ValidarItemCarrinho(ItemProdutoDTO produto, int quantidade)
        {
            if (produto == null) AdicionarErroProcessamento("Produto não existe.");
            if (quantidade < 1) AdicionarErroProcessamento($"Escolha ao menos uma unidade do produto {produto.Nome}");

            var carrinho = await _carrinhoService.ObterCarrinho();
            var itemCarrinho = carrinho.Itens.FirstOrDefault(x => x.ProdutoId == produto.Id);

            if(itemCarrinho != null && itemCarrinho.Quantidade + quantidade > produto.QuantidadeEstoque)
            {
                AdicionarErroProcessamento($"O produto {produto.Nome} possui {produto.QuantidadeEstoque} unidades em estoque. " +
                    $"Você selecionou {quantidade}.");
            }

            if (quantidade > produto.QuantidadeEstoque) 
                AdicionarErroProcessamento($"O produto {produto.Nome} possui {produto.QuantidadeEstoque} unidades em estoque. Você selecionou {quantidade}.");
        }
    }
}
