namespace Venice.Orders.Application.Interfaces
{
    public interface IMensageriaService
    {
        Task PublicarMensagem(object mensagem, string nomeFila);
    }
}
