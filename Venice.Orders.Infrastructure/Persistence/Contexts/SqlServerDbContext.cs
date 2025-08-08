using Microsoft.EntityFrameworkCore;
using Venice.Orders.Domain.Entities;

namespace Venice.Orders.Infrastructure.Persistence.Contexts
{
    public class SqlServerDbContext : DbContext
    {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options) { }

        public DbSet<Pedido> Pedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Requisito: itens do pedido fiquem no MongoDB.
            // EF IGNORAR a propriedade 'Itens' da entidade Pedido para não criar uma tabela no SQL.
            modelBuilder.Entity<Pedido>().Ignore(p => p.Itens);
        }
    }
}
