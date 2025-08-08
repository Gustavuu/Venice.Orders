using Microsoft.AspNetCore.Mvc;
using Venice.Orders.API.Models;
using Venice.Orders.API.Token;

namespace Venice.Orders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;

        public AuthController(ITokenService tokenService, IConfiguration config) 
        {
            _tokenService = tokenService;
            _config = config;
        }

        /// <summary>
        /// Realiza a autenticação de um usuário e retorna um token JWT.
        /// </summary>
        /// <param name="model">Credenciais do usuário (username = test_user e password = password123).</param>
        /// <returns>Um token JWT válido.</returns>
        /// <response code="200">Retorna o token de acesso.</response>
        /// <response code="401">Credenciais inválidas.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // Lógica de autenticação "fake" para o teste, capturando username e password do appsettings
            if (model.Username == _config["FakeAuth:Username"] &&
                model.Password == _config["FakeAuth:Password"])
            {
                var token = _tokenService.GenerateToken("user-123"); // ID do usuário "fake"
                return Ok(new { Token = token });
            }

            return Unauthorized("Usuário ou senha inválidos.");
        }
    }    
}
