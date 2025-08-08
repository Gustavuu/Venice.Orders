using Moq;
using Shouldly;
using Venice.Orders.Application.Features.Pedidos.Commands;
using Venice.Orders.Application.Interfaces;
using Venice.Orders.Domain.Entities;

namespace Venice.Orders.Tests
{
    public class CriarPedidoCommandHandlerTests
    {
        private readonly Mock<IPedidoWriteRepository> _mockWriteRepo;
        private readonly Mock<IMensageriaService> _mockMessagingService;
        private readonly CriarPedidoCommandHandler _criarPedidoCommandHandler;

        public CriarPedidoCommandHandlerTests()
        {
            // Mock para as dependências externas.
            _mockWriteRepo = new Mock<IPedidoWriteRepository>();
            _mockMessagingService = new Mock<IMensageriaService>();

            // Injeta os mocks no handler.
            _criarPedidoCommandHandler = new CriarPedidoCommandHandler(_mockWriteRepo.Object, _mockMessagingService.Object);
        }

        [Fact]
        public async Task Handle_Deve_ChamarRepositorio_E_PublicarMensagem_Quando_Comando_For_Valido()
        {
            // Prepara request
            var request = new CriarPedidoCommand(
                Guid.NewGuid(),
                new List<ItemPedidoDto> { new(Guid.NewGuid(), "Teste", 1, 10) });

            // Executa Handle 
            var pedidoId = await _criarPedidoCommandHandler.Handle(request, CancellationToken.None);

            // Verifica se um ID foi retornado.
            pedidoId.ShouldNotBe(Guid.Empty); 

            // Verifica se o método Adicionar do repositório foi chamado exatamente uma vez.
            _mockWriteRepo.Verify(r => r.Adicionar(It.IsAny<Pedido>()), Times.Once);

            // Verifica se o método PublicarMensagem foi chamado exatamente uma vez.
            _mockMessagingService.Verify(s => s.PublicarMensagem(It.IsAny<object>(), "pedidos-criados"), Times.Once);
        }
    }
}
