using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Services;
using System;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Controllers
{
    public class CatalogoController : MainController
    {
        private readonly ICatalogoService catalogoService;

        public CatalogoController(ICatalogoService catalogoService)
        {
            this.catalogoService = catalogoService;
        }

        [HttpGet]
        [Route("")]
        [Route("vitrine")]
        public async Task<IActionResult> Index([FromQuery] int ps = 4, int pi = 1, string q = null)
        {
            var produtos = await this.catalogoService.ObterTodos(ps, pi, q);

            ViewBag.Pesquisa = q;
            produtos.ReferenceAction = "Index";

            return View(produtos);
        }

        [HttpGet]
        [Route("produto-detalhe/{id}")]
        public async Task<IActionResult> ProdutoDetalhe(Guid id)
        {
            var produto = await this.catalogoService.ObterPorId(id);

            return View(produto);
        }
    }
}
