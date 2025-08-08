using MediatR;
using Venice.Orders.Application.Features.Pedidos.DTO;

namespace Venice.Orders.Application.Features.Pedidos.Queries
{
    public record ObterPedidoPorIdQuery(Guid Id) : IRequest<PedidoResponseDto?>;
}
