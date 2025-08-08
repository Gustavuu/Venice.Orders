using Venice.Orders.Application.Features.Pedidos.Commands;

namespace Venice.Orders.Application.Features.Pedidos.DTO
{
    public record PedidoResponseDto(
        Guid Id,
        Guid ClienteId,
        DateTime DataPedido,
        string Status,
        decimal ValorTotal,
        List<ItemPedidoDto> Itens);
}
