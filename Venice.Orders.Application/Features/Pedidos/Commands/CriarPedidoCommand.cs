using MediatR;

namespace Venice.Orders.Application.Features.Pedidos.Commands;

// O comando implementa IRequest<Guid>, significando que ele espera
// um Guid (o ID do novo pedido) como resposta.
public record CriarPedidoCommand(
    Guid ClienteId,
    List<ItemPedidoDto> Itens) : IRequest<Guid>;

// DTO que representa os dados de um item vindo da requisição da API.
// É importante não usar a entidade do domínio aqui para não vazar detalhes.
public record ItemPedidoDto(
    Guid ProdutoId,
    string DescricaoProduto,
    int Quantidade,
    decimal PrecoUnitario);