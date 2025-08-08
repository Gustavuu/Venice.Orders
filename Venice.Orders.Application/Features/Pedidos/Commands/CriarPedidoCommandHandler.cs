using MediatR;
using Venice.Orders.Application.Interfaces;
using Venice.Orders.Domain.Entities;

namespace Venice.Orders.Application.Features.Pedidos.Commands;

public class CriarPedidoCommandHandler : IRequestHandler<CriarPedidoCommand, Guid>
{
    private readonly IPedidoWriteRepository _pedidoRepository;
    private readonly IMensageriaService _mensageriaService;

    public CriarPedidoCommandHandler(IPedidoWriteRepository pedidoRepository, IMensageriaService mensageriaService)
    {
        _pedidoRepository = pedidoRepository;
        _mensageriaService = mensageriaService;
    }

    public async Task<Guid> Handle(CriarPedidoCommand request, CancellationToken cancellationToken)
    {
        // 1. Cria a entidade de domínio 'Pedido' a partir dos dados do comando
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

        // 2. Chama o repositório para persistir o pedido.
        await _pedidoRepository.Adicionar(pedido);

        // 3. Publica o evento de 'PedidoCriado' na fila.
        var eventoPedidoCriado = new { PedidoId = pedido.Id, Valor = pedido.ValorTotal };
        await _mensageriaService.PublicarMensagem(eventoPedidoCriado, "pedidos-criados");

        // 4. Retornar o ID do pedido recém-criado
        return pedido.Id;
    }
}
