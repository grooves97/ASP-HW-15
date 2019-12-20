using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTAuth.DataAccess;
using JWTAuth.DTOs;
using JWTAuth.Models;
using JWTAuth.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService authService;
        private readonly UserContext userContext;

        public AuthController(AuthService authService, UserContext userContext)
        {
            this.authService = authService;
            this.userContext = userContext;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(AuthDTO authDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var token = await authService.Authenticate(authDTO.Login, authDTO.Password);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            return Ok(new { token });
        }

        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingUser = await userContext.Users.FirstOrDefaultAsync(x => x.Username == user.Username);

            if (existingUser != null)
            {
                return BadRequest();
            }

            userContext.Users.Add(user);
            await userContext.SaveChangesAsync();

            return Ok(new { message = "User successfully registered"});
            
        }
    }
}