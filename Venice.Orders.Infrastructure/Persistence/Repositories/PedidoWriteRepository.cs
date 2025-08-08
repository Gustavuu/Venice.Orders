using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Venice.Orders.Application.Interfaces;
using Venice.Orders.Domain.Entities;
using Venice.Orders.Infrastructure.Persistence.Contexts;

namespace Venice.Orders.Infrastructure.Persistence.Repositories
{
    public class PedidoWriteRepository : IPedidoWriteRepository
    {
        private readonly SqlServerDbContext _sqlContext;
        private readonly IMongoDatabase _mongoDatabase;

        public PedidoWriteRepository(SqlServerDbContext sqlContext, IMongoDatabase mongoDatabase)
        {
            _sqlContext = sqlContext;
            _mongoDatabase = mongoDatabase;
        }

        public async Task Adicionar(Pedido pedido)
        {
            // 1. Adiciona o cabeçalho do pedido ao contexto do SQL Server
            await _sqlContext.Pedidos.AddAsync(pedido);

            // 2. Salva a lista de itens em uma coleção no MongoDB
            if (pedido.Itens.Any())
            {
                var itensCollection = _mongoDatabase.GetCollection<PedidoItensDocument>("itens_pedido");
                var pedidoItensDocument = new PedidoItensDocument(pedido.Id, pedido.Itens.ToList());
                await itensCollection.InsertOneAsync(pedidoItensDocument);
            }

            // 3. Salva as mudanças no SQL Server
            await _sqlContext.SaveChangesAsync();
        }
    }

    // Classe auxiliar para representar o documento que será salvo no MongoDB
    internal class PedidoItensDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public Guid PedidoId { get; set; }
        public List<ItemPedido> Itens { get; set; }

        // Construtor para facilitar a criação no repositório de escrita
        public PedidoItensDocument(Guid pedidoId, List<ItemPedido> itens)
        {
            PedidoId = pedidoId;
            Itens = itens;
        }
    }
}
