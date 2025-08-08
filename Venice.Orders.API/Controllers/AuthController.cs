using Microsoft.AspNetCore.Mvc;
using Venice.Orders.API.Token;

namespace Venice.Orders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // Lógica de autenticação "fake" para o teste
            if (model.Username == "venice" && model.Password == "orders123")
            {
                var token = _tokenService.GenerateToken("user-123"); // ID do usuário "fake"
                return Ok(new { Token = token });
            }

            return Unauthorized("Usuário ou senha inválidos.");
        }
    }

    public record LoginModel(string Username, string Password);
}
