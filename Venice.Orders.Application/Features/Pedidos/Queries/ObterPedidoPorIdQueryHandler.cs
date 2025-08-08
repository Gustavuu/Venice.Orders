using MediatR;
using Venice.Orders.Application.Features.Pedidos.DTO;
using Venice.Orders.Application.Interfaces;

namespace Venice.Orders.Application.Features.Pedidos.Queries
{
    public class ObterPedidoPorIdQueryHandler : IRequestHandler<ObterPedidoPorIdQuery, PedidoResponseDto?>
    {
        private readonly IPedidoReadRepository _readRepository;
        private readonly ICacheService _cacheService;
        private const string CacheKeyPrefix = "pedido:";

        public ObterPedidoPorIdQueryHandler(IPedidoReadRepository readRepository, ICacheService cacheService)
        {
            _readRepository = readRepository;
            _cacheService = cacheService;
        }

        public async Task<PedidoResponseDto?> Handle(ObterPedidoPorIdQuery request, CancellationToken cancellationToken)
        {
            // Define uma chave única para este pedido no cache
            var cacheKey = $"{CacheKeyPrefix}{request.Id}";

            // 1. Tenta buscar do cache primeiro
            var cachedPedido = await _cacheService.GetAsync<PedidoResponseDto>(cacheKey);

            if (cachedPedido is not null)
            {
                // Cache HIT! Retorna o dado do cache.
                return cachedPedido;
            }

            // 2. Cache MISS. Busca do repositório (banco de dados)
            var pedido = await _readRepository.ObterPorIdAsync(request.Id);

            // 3. Se encontrou no banco, salva no cache antes de retornar
            if (pedido is not null)
            {
                await _cacheService.SetAsync(cacheKey, pedido, TimeSpan.FromMinutes(2)); //Pode ser uma variável de ambiente
            }

            return pedido;
        }
    }
}
