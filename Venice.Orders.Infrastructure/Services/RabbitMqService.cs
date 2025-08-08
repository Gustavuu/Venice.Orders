using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Venice.Orders.Application.Interfaces;

namespace Venice.Orders.Infrastructure.Services
{
    public class RabbitMqService : IMensageriaService
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqService(string connectionString)
        {
            // O construtor agora apenas cria e armazena a fábrica de conexões.
            _factory = new ConnectionFactory() { Uri = new Uri(connectionString) };
        }

        // O método de publicação agora é assíncrono.
        public async Task PublicarMensagem(object mensagem, string nomeFila)
        {
            // Usamos 'await using' para garantir que a conexão e o canal
            // sejam criados, usados e descartados corretamente a cada chamada.
            // Isso é mais resiliente do que manter uma conexão aberta.
            await using var connection = await _factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            // O resto da lógica permanece o mesmo.
            await channel.QueueDeclareAsync(queue: nomeFila,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var mensagemJson = JsonSerializer.Serialize(mensagem);
            var body = Encoding.UTF8.GetBytes(mensagemJson);

            await channel.BasicPublishAsync(exchange: "",
                                     routingKey: nomeFila,
                                     mandatory: false,
                                     body: body);
        }
    }
}
