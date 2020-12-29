using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace NSE.WebApi.Core.Usuario
{
    public class AspNetUser : IAspNetUser
    {
        private readonly IHttpContextAccessor accessor;

        public AspNetUser(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }
        public string Name => this.accessor.HttpContext.User.Identity.Name;

        public bool EstaAutenticado()
        {
            return this.accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public IEnumerable<Claim> ObterClaims()
        {
            return this.accessor.HttpContext.User.Claims;
        }

        public HttpContext ObterHttpContext()
        {
            return this.accessor.HttpContext;
        }

        public string ObterUserEmail()
        {
            return EstaAutenticado() ? this.accessor.HttpContext.User.GetUserEmail() : string.Empty;
        }

        public Guid ObterUserId()
        {
            return EstaAutenticado() ? Guid.Parse(this.accessor.HttpContext.User.GetUserId()) : Guid.Empty;
        }

        public string ObterUserToken()
        {
            return EstaAutenticado() ? this.accessor.HttpContext.User.GetUserToken() : string.Empty;
        }

        public bool PossuiRole(string role)
        {
            return this.accessor.HttpContext.User.IsInRole(role);
        }
    }
}
