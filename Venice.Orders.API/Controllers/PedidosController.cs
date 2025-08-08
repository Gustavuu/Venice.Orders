using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Venice.Orders.Application.Features.Pedidos.Commands;
using Venice.Orders.Application.Features.Pedidos.DTO;
using Venice.Orders.Application.Features.Pedidos.Queries;

namespace Venice.Orders.API.Controllers
{
    [Authorize]
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
            return CreatedAtAction(nameof(GetPedidoPorId), new { id = pedidoId }, new { Id = pedidoId });
        }

        [HttpGet("{id:guid}")] // Adicionamos :guid para garantir que o id seja um Guid válido
        [ProducesResponseType(typeof(PedidoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPedidoPorId(Guid id)
        {
            var query = new ObterPedidoPorIdQuery(id);
            var pedido = await _mediator.Send(query);

            return pedido is not null ? Ok(pedido) : NotFound();
        }
    }
}
