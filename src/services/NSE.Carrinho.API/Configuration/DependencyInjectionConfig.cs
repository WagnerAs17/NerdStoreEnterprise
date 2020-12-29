using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSE.Carrinho.API.Business;
using NSE.Carrinho.API.Data;
using NSE.Carrinho.API.Data.Repository;
using NSE.WebApi.Core.Usuario;

namespace NSE.Carrinho.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();
            services.AddScoped<CarrinhoContext>();

            BusinessInjection(services);
            RepositoryInjection(services);
        }

        private static void BusinessInjection(IServiceCollection services)
        {
            services.AddScoped<ICarrinhoBusiness, CarrinhoBusiness>();
        }

        private static void RepositoryInjection(IServiceCollection services)
        {
            services.AddScoped<ICarrinhoRepository, CarrinhoRepository>();
        }
    }
}
