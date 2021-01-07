using NSE.Core.Communication;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NSE.BFF.Compras.Services
{
    public abstract class Service
    {
        protected StringContent ObterDado(object dado)
        {
            return new StringContent
                (
                    JsonSerializer.Serialize(dado),
                    Encoding.UTF8,
                    "application/json"
                 );
        }

        protected async Task<T> DeserializarObjetoResponse<T>(HttpResponseMessage response)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true
            };

            return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), options);
        }

        protected bool TratarErrosResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest) return false;

            response.EnsureSuccessStatusCode();
            return true;
        }

        protected ResponseResult ReturnOK()
        {
            return new ResponseResult();
        }
    }
}
