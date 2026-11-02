using ECommerce.Entities.DTOs;
using ECommerce.Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]

            public async Task<IActionResult> Login(LoginDto loginDto)
            {
                var token = await _authService.LoginAsync(loginDto);

                if (token == null)
                {
                    return Unauthorized("Email or password is incorrect.");
                }

                return Ok(new
                {
                    token = token
                });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (!result)
            {
                return BadRequest("Email already exists.");
            }

            return Ok("User registered successfully.");
        }
    }
}