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
            _factory = new ConnectionFactory() { Uri = new Uri(connectionString) };
        }

        public async Task PublicarMensagem(object mensagem, string nomeFila)
        {
            await using var connection = await _factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

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
