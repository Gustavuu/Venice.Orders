using MediatR;
using Venice.Orders.Application.Features.Pedidos.DTO;
using Venice.Orders.Application.Interfaces;

namespace Venice.Orders.Application.Features.Pedidos.Queries
{
    public class ObterPedidoPorIdQueryHandler : IRequestHandler<ObterPedidoPorIdQuery, PedidoResponseDto?>
    {
        private readonly IPedidoReadRepository _readRepository;

        public ObterPedidoPorIdQueryHandler(IPedidoReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<PedidoResponseDto?> Handle(ObterPedidoPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _readRepository.ObterPorIdAsync(request.Id);
        }
    }
}
