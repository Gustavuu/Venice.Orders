using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Venice.Orders.Application.Features.Pedidos.Commands;
using Venice.Orders.Application.Features.Pedidos.DTO;
using Venice.Orders.Application.Interfaces;
using Venice.Orders.Infrastructure.Persistence.Contexts;

namespace Venice.Orders.Infrastructure.Persistence.Repositories
{
    public class PedidoReadRepository : IPedidoReadRepository
    {
        private readonly SqlServerDbContext _sqlContext;
        private readonly IMongoDatabase _mongoDatabase;

        public PedidoReadRepository(SqlServerDbContext sqlContext, IMongoDatabase mongoDatabase)
        {
            _sqlContext = sqlContext;
            _mongoDatabase = mongoDatabase;
        }

        public async Task<PedidoResponseDto?> ObterPorIdAsync(Guid id)
        {
            // 1. Buscar os dados principais do pedido no SQL Server
            var pedido = await _sqlContext.Pedidos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido is null)
            {
                return null; // Retorna nulo se o pedido não for encontrado
            }

            // 2. Buscar os itens do pedido no MongoDB
            var itensCollection = _mongoDatabase.GetCollection<PedidoItensDocument>("itens_pedido");
            var pedidoItensDocument = await itensCollection.Find(doc => doc.PedidoId == id).FirstOrDefaultAsync();

            var itensDto = pedidoItensDocument?.Itens
                .Select(item => new ItemPedidoDto(
                    item.ProdutoId,
                    item.DescricaoProduto,
                    item.Quantidade,
                    item.PrecoUnitario))
                .ToList() ?? new List<ItemPedidoDto>();

            // 3. Combinar os dados em um único DTO de resposta
            var response = new PedidoResponseDto(
                pedido.Id,
                pedido.ClienteId,
                pedido.DataPedido,
                pedido.Status.ToString(),
                pedido.ValorTotal,
                itensDto);

            return response;
        }
    }
}
