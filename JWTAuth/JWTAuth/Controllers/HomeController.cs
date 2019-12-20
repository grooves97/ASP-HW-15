using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWTAuth.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly string secretOptions;

        public HomeController(IOptions<SecretOption> secretOptions)
        {
            this.secretOptions = secretOptions.Value.JWTSecret;
        }

        public IActionResult GetSecureInfo()
        {
            var key = Encoding.ASCII.GetBytes(secretOptions);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false

            };

            var token = HttpContext.Request.Headers["Authorization"];
            token = token.ToString().Replace("Bearer", "");

            var claims = handler.ValidateToken(token, validations, out var tokenSecure);

            return Ok(new { name = claims.Identity.Name});
        }
    }
}