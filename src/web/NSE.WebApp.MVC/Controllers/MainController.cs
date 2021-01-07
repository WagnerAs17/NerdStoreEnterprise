using Microsoft.AspNetCore.Mvc;
using NSE.Core.Communication;
using System.Linq;

namespace NSE.WebApp.MVC.Controllers
{
    public class MainController : Controller
    {
        protected bool ResponsePossuiErros(ResponseResult responseResult)
        {
            if(responseResult != null && responseResult.Errors.Mensagens.Any())
            {
                foreach (var error in responseResult.Errors.Mensagens)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return true;
            }

            return false;
        }

        protected void AdicionarErroValidacao(string message)
        {
            ModelState.AddModelError(string.Empty, message);
        }

        protected bool OperacaoValida()
        {
            return ModelState.ErrorCount == 0;
        }
    }
}
