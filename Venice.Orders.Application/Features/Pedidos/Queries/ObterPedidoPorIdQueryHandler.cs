using MediatR;
using Microsoft.Extensions.Configuration;
using Venice.Orders.Application.Features.Pedidos.DTO;
using Venice.Orders.Application.Interfaces;

namespace Venice.Orders.Application.Features.Pedidos.Queries
{
    public class ObterPedidoPorIdQueryHandler : IRequestHandler<ObterPedidoPorIdQuery, PedidoResponseDto?>
    {
        private readonly IPedidoReadRepository _readRepository;
        private readonly ICacheService _cacheService;
        private const string CacheKeyPrefix = "pedido:";
        private readonly IConfiguration _configuration;

        public ObterPedidoPorIdQueryHandler(IPedidoReadRepository readRepository, ICacheService cacheService, IConfiguration configuration)
        {
            _readRepository = readRepository;
            _cacheService = cacheService;
            _configuration = configuration;
        }

        public async Task<PedidoResponseDto?> Handle(ObterPedidoPorIdQuery request, CancellationToken cancellationToken)
        {
            // Define uma chave única para este pedido no cache
            var cacheKey = $"{CacheKeyPrefix}{request.Id}";

            // 1. Tenta buscar do cache primeiro
            var cachedPedido = await _cacheService.GetAsync<PedidoResponseDto>(cacheKey);

            if (cachedPedido is not null)
            {
                // Retorna o dado do cache.
                return cachedPedido;
            }

            // 2. Cache não encontrado / Busca do repositório
            var pedido = await _readRepository.ObterPorIdAsync(request.Id);

            // 3. Se encontrou no banco, salva no cache antes de retornar
            if (pedido is not null)
            {
                var expirationMinutesString = _configuration.GetSection("CacheSettings:ExpirationMinutes").Value;

                if (!int.TryParse(expirationMinutesString, out var expirationMinutes))
                {
                    expirationMinutes = 2;
                }

                await _cacheService.SetAsync(cacheKey, pedido, TimeSpan.FromMinutes(expirationMinutes));
            }

            return pedido;
        }
    }
}
