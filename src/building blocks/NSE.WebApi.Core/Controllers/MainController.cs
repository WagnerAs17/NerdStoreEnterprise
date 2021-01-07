using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSE.Core.Communication;
using System.Collections.Generic;
using System.Linq;

namespace NSE.WebApi.Core.Controllers
{
    [ApiController]
    public abstract class MainController : Controller
    {
        protected List<string> Erros = new List<string>();
        protected IActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
                return Ok(result);

            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Mensagens", Erros.ToArray() }
            }));
        }

        protected IActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(er => er.Errors);

            foreach (var erro in erros)
            {
                AdicionarErroProcessamento(erro.ErrorMessage);
            }

            return CustomResponse();
        }

        protected IActionResult CustomResponse(ValidationResult validationResult)
        {
            foreach (var erro in validationResult.Errors)
            {
                AdicionarErroProcessamento(erro.ErrorMessage);
            }

            return CustomResponse();
        }

        protected IActionResult CustomResponse(ResponseResult response)
        {
            ResponsePossuiErros(response);

            return CustomResponse();
        }

        protected bool ResponsePossuiErros(ResponseResult response)
        {
            if (response != null || response.Errors.Mensagens.Any()) return false;

            foreach (var message in response.Errors.Mensagens)
            {
                AdicionarErroProcessamento(message);
            }

            return true;
        }

        protected bool OperacaoValida()
        {
            return !Erros.Any();
        }

        protected void AdicionarErroProcessamento(string erro)
        {
            Erros.Add(erro);
        }

        protected void LimparProcessamento()
        {
            Erros.Clear();
        }
    }
}
