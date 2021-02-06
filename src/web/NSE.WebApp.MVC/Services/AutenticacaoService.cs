using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using NSE.Core.Communication;
using NSE.WebApi.Core.Usuario;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public class AutenticacaoService : Service, IAutenticacaoService
    {
        private readonly HttpClient httpClient;
        public readonly IAspNetUser _aspNetUser;
        private readonly IAuthenticationService _authenticationService;

        public AutenticacaoService
        (
            HttpClient httpClient,
            IOptions<AppSettings> settings,
            IAuthenticationService authenticationService,
            IAspNetUser aspNetUser
        )
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri(settings.Value.AutenticacaoUrl);
            _aspNetUser = aspNetUser;
            _authenticationService = authenticationService;
        }

        public async Task<UsuarioRepostaLogin> Login(UsuarioLogin usuarioLogin)
        {
            var loginContent = ObterConteudo(usuarioLogin);

            var response = await httpClient.PostAsync("/api/identidade/autenticar", loginContent);

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRepostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }

            return await DeserializarObjetoResponse<UsuarioRepostaLogin>(response);
        }

        public async Task<UsuarioRepostaLogin> Registro(UsuarioRegistro usuarioRegistro)
        {
            var registroContent = ObterConteudo(usuarioRegistro);

            var response = await this.httpClient.PostAsync("/api/identidade/nova-conta", registroContent);

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRepostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }

            return await DeserializarObjetoResponse<UsuarioRepostaLogin>(response);
        }

        private async Task<UsuarioRepostaLogin> UtilizarRefreshToken(string refreshToken)
        {
            var refreshTokenContent = ObterConteudo(refreshToken);

            var response = await httpClient.PostAsync("api/identidade/refresh-token", refreshTokenContent);

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRepostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }

            return await DeserializarObjetoResponse<UsuarioRepostaLogin>(response);
        }

        public async Task<bool> RefreshTokenValido()
        {
            var response = await UtilizarRefreshToken(_aspNetUser.ObterUserRefreshToken());

            if(response.RefreshToken != null && response.ResponseResult == null)
            {
                await RealizarLogin(response);
                return true;
            }

            return true;
        }

        public async Task RealizarLogin(UsuarioRepostaLogin resposta)
        {
            var token = ObterTokenFormatado(resposta.AccessToken);

            var claims = new List<Claim>();
            claims.Add(new Claim("Jwt", resposta.AccessToken));
            claims.Add(new Claim("RefreshToken", resposta.RefreshToken));
            claims.AddRange(token.Claims);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                IsPersistent = true
            };

            await _authenticationService.SignInAsync(
                _aspNetUser.ObterHttpContext(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public async Task Logout()
        {
            await _authenticationService.SignOutAsync(
                _aspNetUser.ObterHttpContext(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                null);
        } 

        private static JwtSecurityToken ObterTokenFormatado(string token)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(token) as JwtSecurityToken;
        }

        public bool TokenExpirado()
        {
            var jwt = _aspNetUser.ObterUserToken();

            if (jwt is null) return false;

            var token = ObterTokenFormatado(jwt);

            return token.ValidTo.ToLocalTime() < DateTime.Now;
        }
    }
}
