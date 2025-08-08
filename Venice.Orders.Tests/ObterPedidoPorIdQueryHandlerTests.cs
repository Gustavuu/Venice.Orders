using Moq;
using Shouldly;
using Venice.Orders.Application.Features.Pedidos.DTO;
using Venice.Orders.Application.Features.Pedidos.Queries;
using Venice.Orders.Application.Interfaces;

namespace Venice.Orders.Tests
{
    public class ObterPedidoPorIdQueryHandlerTests
    {
        private readonly Mock<IPedidoReadRepository> _mockReadRepo;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly ObterPedidoPorIdQueryHandler _obterPedidoPorIdQueryhandler;

        public ObterPedidoPorIdQueryHandlerTests()
        {
            // Mock para as dependências externas
            _mockReadRepo = new Mock<IPedidoReadRepository>();
            _mockCacheService = new Mock<ICacheService>();

            // Injeta os mocks no handler
            _obterPedidoPorIdQueryhandler = new ObterPedidoPorIdQueryHandler(_mockReadRepo.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task Handle_Deve_RetornarPedidoDoCache_E_NaoChamarRepositorio_Quando_PedidoExisteNoCache()
        {
            // Prepara variáveis
            var pedidoId = Guid.NewGuid();
            var query = new ObterPedidoPorIdQuery(pedidoId);
            var pedidoEmCache = new PedidoResponseDto(pedidoId, Guid.NewGuid(), DateTime.Now, "Faturado", 100, new());

            // Redis Mock
            _mockCacheService
                .Setup(c => c.GetAsync<PedidoResponseDto>($"pedido:{pedidoId}"))
                .ReturnsAsync(pedidoEmCache);

            // Executa Handle 
            var resultado = await _obterPedidoPorIdQueryhandler.Handle(query, CancellationToken.None);

            // Verificações
            resultado.ShouldNotBeNull();
            resultado.Id.ShouldBe(pedidoId); // Verifica se o ID está correto.
            resultado.ShouldBe(pedidoEmCache); // Verifica se é o mesmo objeto do cache.

            // Verifica se o método do repositório NUNCA foi chamado, provando que a lógica de cache funcionou.
            _mockReadRepo.Verify(r => r.ObterPorIdAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
