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

        /// <summary>
        /// Cria um novo pedido no sistema.
        /// </summary>
        /// <param name="command">Dados para a criação do novo pedido.</param>
        /// <returns>O ID do pedido recém-criado.</returns>
        /// <response code="201">Pedido criado com sucesso.</response>
        /// <response code="400">Dados inválidos para a criação do pedido.</response>
        /// <response code="401">Não autorizado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoCommand command)
        {
            var pedidoId = await _mediator.Send(command);

            // Retorna um HTTP 201 Created
            return CreatedAtAction(nameof(GetPedidoPorId), new { id = pedidoId }, new { Id = pedidoId });
        }

        /// <summary>
        /// Busca um pedido específico pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do pedido a ser consultado.</param>
        /// <returns>Os dados detalhados do pedido.</returns>
        /// <response code="200">Retorna os dados do pedido.</response>
        /// <response code="404">Pedido não encontrado.</response>
        /// <response code="401">Não autorizado.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PedidoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPedidoPorId(Guid id)
        {
            var query = new ObterPedidoPorIdQuery(id);
            var pedido = await _mediator.Send(query);

            return pedido is not null ? Ok(pedido) : NotFound();
        }
    }
}
