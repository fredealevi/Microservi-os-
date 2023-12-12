using ItemService.EventProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ItemService.RabbitMqClient
{
    public class RabbitMqSubscriber : BackgroundService
    {
        //configuração
        private readonly IConfiguration _configuration;
        //conexão
        private readonly IConnection _connection;
        //modelo do canal
        private readonly IModel _channel;

        private readonly string _nomeDaFila;
        private readonly IProcessaEvento _processaEvento;

        public RabbitMqSubscriber(IConfiguration configuration, IProcessaEvento processaEvento)
        {
            _configuration = configuration;

            _connection = new ConnectionFactory()
            // cria uma conexão RabbitMqHost na porta 8200
            { HostName = _configuration["RabbitMqHost"], Port = Int32.Parse(_configuration["RabbitMqPort"]) }.CreateConnection();
            //cria um moelo de conexão
            _channel = _connection.CreateModel();
            //medole de conexão ao iniciar
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            // pegando o nome da fila atraves do canal 
            _nomeDaFila = _channel.QueueDeclare().QueueName;
            //Ligando a fila ao canal 
            _channel.QueueBind(queue: _nomeDaFila, exchange: "trigger", routingKey: "");
            _processaEvento = processaEvento;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumidor = new EventingBasicConsumer(_channel);

            consumidor.Received += (ModuleHandle, evento) =>
            {
                var body = evento.Body;
                var mensagem = Encoding.UTF8.GetString(body.ToArray());

                _processaEvento.Processa(mensagem);
            };
            // informo ao Rabbit que o dado já foi consumido (processado)
            _channel.BasicConsume(queue: _nomeDaFila, autoAck: true, consumer: consumidor);

            // retorna que a tarefa esta completa.
            return Task.CompletedTask;
        }
    }
}
