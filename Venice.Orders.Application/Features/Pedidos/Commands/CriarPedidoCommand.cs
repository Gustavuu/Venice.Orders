using MediatR;

namespace Venice.Orders.Application.Features.Pedidos.Commands;

// Através de IRequest<Guid> retorna um Guid (o ID do novo pedido) como resposta.
public record CriarPedidoCommand(
    Guid ClienteId,
    List<ItemPedidoDto> Itens) : IRequest<Guid>;

// DTO que representa os dados de um item vindo da requisição da API.
public record ItemPedidoDto(
    Guid ProdutoId,
    string DescricaoProduto,
    int Quantidade,
    decimal PrecoUnitario);