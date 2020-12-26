using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;
using System.Linq;

namespace NSE.WebApp.MVC.Controllers
{
    public class MainController : Controller
    {
        protected bool ResponsePossuiErros(ResponseResult responseResult)
        {
            if(responseResult != null && responseResult.Errors.Mensagem.Any())
            {
                foreach (var error in responseResult.Errors.Mensagem)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return true;
            }

            return false;
        }
    }
}
