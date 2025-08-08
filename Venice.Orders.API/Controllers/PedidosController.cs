using MediatR;
using Microsoft.AspNetCore.Mvc;
using Venice.Orders.Application.Features.Pedidos.Commands;

namespace Venice.Orders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PedidosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoCommand command)
        {
            var pedidoId = await _mediator.Send(command);

            // Retorna um HTTP 201 Created com o link para o novo recurso (que criaremos depois)
            return CreatedAtAction(nameof(GetPedidoPorId), new { id = pedidoId }, command);
        }

        // Endpoint placeholder apenas para o CreatedAtAction funcionar.
        // Vamos implementá-lo de verdade mais tarde.
        [HttpGet("{id}")]
        public IActionResult GetPedidoPorId(Guid id)
        {
            return Ok();
        }
    }
}
