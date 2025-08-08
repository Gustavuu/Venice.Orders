using Venice.Orders.Domain.Enums;

namespace Venice.Orders.Domain.Entities
{
    public class Pedido
    {
        public Guid Id { get; private set; }
        public Guid ClienteId { get; private set; }
        public DateTime DataPedido { get; private set; }
        public StatusPedido Status { get; private set; }
        public decimal ValorTotal { get; private set; }

        // IReadOnlyCollection para proteger a lista de modificações externas diretas.
        private readonly List<ItemPedido> _itens = new();
        public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

        public Pedido(Guid clienteId)
        {
            Id = Guid.NewGuid();
            ClienteId = clienteId;
            DataPedido = DateTime.UtcNow;
            Status = StatusPedido.Pendente;
            ValorTotal = 0;
        }

        public void AdicionarItem(ItemPedido item)
        {
            _itens.Add(item);
            CalcularValorTotal();
        }

        // Método de negócio para calcular o valor total.
        private void CalcularValorTotal()
        {
            ValorTotal = Itens.Sum(item => item.Quantidade * item.PrecoUnitario);
        }

        // Métodos para mudar o status
        public void MarcarComoFaturado()
        {
            Status = StatusPedido.Faturado;
        }

        public void MarcarComoCancelado()
        {
            Status = StatusPedido.Cancelado;
        }

        // O construtor vazio é para uso do Entity Framework.
        protected Pedido() { }
    }
}
