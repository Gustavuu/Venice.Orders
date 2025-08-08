using MediatR;
using Venice.Orders.Application.Interfaces;
using Venice.Orders.Domain.Entities;

namespace Venice.Orders.Application.Features.Pedidos.Commands;

public class CriarPedidoCommandHandler : IRequestHandler<CriarPedidoCommand, Guid>
{
    private readonly IPedidoWriteRepository _pedidoRepository;
    private readonly IMensageriaService _mensageriaService;

    // As dependências são injetadas via construtor (Dependency Injection)
    public CriarPedidoCommandHandler(IPedidoWriteRepository pedidoRepository, IMensageriaService mensageriaService)
    {
        _pedidoRepository = pedidoRepository;
        _mensageriaService = mensageriaService;
    }

    public async Task<Guid> Handle(CriarPedidoCommand request, CancellationToken cancellationToken)
    {
        // 1. Criar a entidade de domínio 'Pedido' a partir dos dados do comando
        var pedido = new Pedido(request.ClienteId);

        foreach (var itemDto in request.Itens)
        {
            var item = new ItemPedido(
                itemDto.ProdutoId,
                itemDto.DescricaoProduto,
                itemDto.Quantidade,
                itemDto.PrecoUnitario);

            pedido.AdicionarItem(item);
        }

        // 2. Chamar o repositório para persistir o pedido.
        // Não sabemos como será salvo (SQL? Mongo?), apenas que o contrato será cumprido.
        await _pedidoRepository.Adicionar(pedido);

        // 3. Publicar o evento de 'PedidoCriado' na fila.
        // Usamos um objeto anônimo por simplicidade, mas poderia ser uma classe de evento dedicada.
        var eventoPedidoCriado = new { PedidoId = pedido.Id, Valor = pedido.ValorTotal };
        await _mensageriaService.PublicarMensagem(eventoPedidoCriado, "pedidos-criados");

        // 4. Retornar o ID do pedido recém-criado
        return pedido.Id;
    }
}
