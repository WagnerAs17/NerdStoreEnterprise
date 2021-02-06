using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Catalogo.API.Models;
using NSE.Core.DomainObjects;
using NSE.Core.Messages.Integrations;
using NSE.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NSE.Catalogo.API.Services
{
    public class CatalogoIntegrationHandler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageBus _bus;

        public CatalogoIntegrationHandler
        (
            IServiceProvider serviceProvider,
            IMessageBus bus
        )
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetSubscribers();
            return Task.CompletedTask;
        }

        private void SetSubscribers()
        {
            _bus.SubscribeAsync<PedidoAutorizadoIntegrationEvent>("PedidoAutorizado", async request =>
                await BaixarEstoque(request));
        }
        
        private async Task BaixarEstoque(PedidoAutorizadoIntegrationEvent message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var produtosComEstoque = new List<Produto>();

                var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();

                var idsProdutos = string.Join(",", message.Itens.Select(c => c.Key));

                var produtos = await produtoRepository.ObterProdutosPorId(idsProdutos);

                if(produtos.Count != message.Itens.Count)
                {
                    CancelarPedidosSemEstoque(message);
                    return;
                }

                foreach (var produto in produtos)
                {
                    var quantidadeProduto = message.Itens.FirstOrDefault(x => x.Key == produto.Id).Value;

                    if (produto.EstaDisponivel(quantidadeProduto))
                    {
                        produto.RetirarEstoque(quantidadeProduto);

                        produtosComEstoque.Add(produto);
                    }
                }

                if(produtosComEstoque.Count != message.Itens.Count)
                {
                    CancelarPedidosSemEstoque(message);
                    return;
                }


                foreach (var produto in produtosComEstoque)
                {
                    produtoRepository.Atualizar(produto);
                }

                if(!await produtoRepository.UnitOfWork.Commit())
                {
                    throw new DomainException($"Problemas para atualizar estoque do pedido {message.PedidoId}");
                }

                var pedidoBaixado = new PedidoBaixadoIntegrationEvent(message.ClienteId, message.PedidoId);

                await _bus.PublishAsync(pedidoBaixado);
            }
        }

        private async void CancelarPedidosSemEstoque(PedidoAutorizadoIntegrationEvent message)
        {
            var pedidoCancelado = new PedidoCanceladoIntegrationEvent(message.ClienteId, message.PedidoId);

            await _bus.PublishAsync(pedidoCancelado);
        }
    }
}
