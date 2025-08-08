namespace Venice.Orders.Domain.Entities
{
    public class ItemPedido
    {
        public Guid ProdutoId { get; private set; }
        public string DescricaoProduto { get; private set; }
        public int Quantidade { get; private set; }
        public decimal PrecoUnitario { get; private set; }

        public ItemPedido(Guid produtoId, string descricaoProduto, int quantidade, decimal precoUnitario)
        {
            ProdutoId = produtoId;
            DescricaoProduto = descricaoProduto;
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
        }
    }
}
