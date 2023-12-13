using RabbitMQ.Client;
using RestauranteService.Dtos;
using System.Text;
using System.Text.Json;

namespace RestauranteService.RabbitMqClient
{
    public class RabbitMqClient : IRabbitMqClient
    {
        //configuração
        private readonly IConfiguration _configuration;
        //conexão
        private readonly IConnection _connection;
        //modelo do canal
        private readonly IModel _channel;

        public RabbitMqClient(IConfiguration configuration)
        {
            _configuration = configuration;

            _connection = new ConnectionFactory()
            // cria uma conexão RabbitMqHost na porta 
            { HostName = _configuration["RabbitMqHost"], Port = Int32.Parse(_configuration["RabbitMqPort"]) }.CreateConnection();
            //cria um moelo de conexão
            _channel = _connection.CreateModel();
            //medole de conexão ao iniciar
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
        }

        public void PublicaRestaurante(RestauranteReadDto restauranteReadDto)
        {
            var mensagem = JsonSerializer.Serialize(restauranteReadDto);

            // transforma a mensagem em bytes
            var body = Encoding.UTF8.GetBytes(mensagem);

            _channel.BasicPublish(
                exchange: "trigger",
                routingKey: "",
                basicProperties: null,
                body: body
                );
        }
    }
}