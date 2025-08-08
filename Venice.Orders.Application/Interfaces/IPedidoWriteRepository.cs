using Venice.Orders.Domain.Entities;

namespace Venice.Orders.Application.Interfaces
{
    public interface IPedidoWriteRepository
    {
        Task Adicionar(Pedido pedido);
    }
}
