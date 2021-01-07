using Microsoft.AspNetCore.Http;
using Polly.CircuitBreaker;
using Refit;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (CustomHttpRequestException ex)
            {
                HandleRequestExceptionAsync(context, ex.StatusCode);
            }
            catch(ValidationApiException ex)
            {
                HandleRequestExceptionAsync(context, ex.StatusCode);
            }
            catch (BrokenCircuitException)
            {
                HandlerCircuitBreakerExceptionAsync(context);
            }
        }

        private static void HandleRequestExceptionAsync(HttpContext context, HttpStatusCode statusCode)
        {
            if(statusCode == HttpStatusCode.Unauthorized)
            {
                context.Response.Redirect($"/login?ReturnUrl={context.Request.Path}");
                return;
            }

            context.Response.StatusCode = (int)statusCode;
        }

        private static void HandlerCircuitBreakerExceptionAsync(HttpContext context)
        {
            context.Response.Redirect("/sistema-indisponivel");
        }
    }
}
