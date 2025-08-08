using Venice.Orders.Application.Features.Pedidos.DTO;

namespace Venice.Orders.Application.Interfaces
{
    public interface IPedidoReadRepository
    {
        Task<PedidoResponseDto?> ObterPorIdAsync(Guid id);
    }
}
