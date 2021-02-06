using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.JwtSigningCredentials.Interfaces;
using NSE.Identidade.API.Data;
using NSE.Identidade.API.Extensions;
using NSE.Identidade.API.Models;
using NSE.MessageBus;
using NSE.WebApi.Core.Identidade;
using NSE.WebApi.Core.Usuario;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NSE.Identidade.API.Services
{
    public class AuthService
    {
        public readonly SignInManager<IdentityUser> SignInManager;
        public readonly UserManager<IdentityUser> UserManager;
        private readonly AppSettings _appSettings;
        private readonly IMessageBus _bus;
        private readonly IAspNetUser _user;
        private readonly IJsonWebKeySetService _jwksService;
        private readonly ApplicationDbContext _context;
        private readonly AppTokenSettings _tokenSettings;

        public AuthService
        (
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<AppSettings> appSettings,
            IMessageBus bus,
            IAspNetUser user,
            IJsonWebKeySetService jwksService,
            ApplicationDbContext context,
            IOptions<AppTokenSettings> tokenSettings
        )
        {
            SignInManager = signInManager;
            UserManager = userManager;
            _appSettings = appSettings.Value;
            _bus = bus;
            _user = user;
            _jwksService = jwksService;
            _context = context;
            _tokenSettings = tokenSettings.Value;
        }

        public async Task<usuarioRespostaLogin> GerarJwt(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);

            var claims = await UserManager.GetClaimsAsync(user);

            var identityClaim = await ObterClaimsUsuario(claims, user);

            var encodedToken = CodificarToken(identityClaim);

            var refreshToken = await GerarRefreshToken(email);

            return ObterRespostaLogin(encodedToken, user, claims, refreshToken);
        }

        private async Task<ClaimsIdentity> ObterClaimsUsuario(ICollection<Claim> claims, IdentityUser user)
        {
            var userRoles = await UserManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            var identityClaim = new ClaimsIdentity();
            identityClaim.AddClaims(claims);

            return identityClaim;
        }

        private string CodificarToken(ClaimsIdentity identityClaim)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var currentIssuer = $"{_user.ObterHttpContext().Request.Scheme}://{_user.ObterHttpContext().Request.Host}";

            var key = _jwksService.GetCurrent();

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = currentIssuer,
                Subject = identityClaim,
                Expires = DateTime.UtcNow.AddHours(1),//padrao é uma hora
                SigningCredentials = key
            });

            return tokenHandler.WriteToken(token);
        }

        private usuarioRespostaLogin ObterRespostaLogin(string encodedToken, IdentityUser user, IList<Claim> claims, RefreshToken refreshToken)
        {
            var response = new usuarioRespostaLogin
            {
                AccessToken = encodedToken,
                ExpireIn = TimeSpan.FromHours(1).TotalSeconds,
                UsuarioToken = new UsuarioToken
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
                },
                RefreshToken = refreshToken.Token
            };

            return response;
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        private async Task<RefreshToken> GerarRefreshToken(string email)
        {
            var refreshToken = new RefreshToken
            {
                UserName = email,
                ExpirationDate = DateTime.UtcNow.AddHours(_tokenSettings.RefreshTokenExpiration)
            };

            _context.RefreshTokens.RemoveRange(_context.RefreshTokens.Where(x => x.UserName == email));

            await _context.RefreshTokens.AddAsync(refreshToken);

            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<RefreshToken> ObterRefreshToken(Guid refreshToken)
        {
            var token = await _context.RefreshTokens.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            return token != null && token.ExpirationDate.ToLocalTime() > DateTime.Now ?
                token : null;
        }
    }
}
