using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace user_service_api.Tasks;

public class MQTask
{
    private readonly IServiceProvider _serviceProvider;
    private IConnection _connection;
    private IModel _channel;
    private readonly List<EventingBasicConsumer> _consumers = new List<EventingBasicConsumer>();
    private readonly List<string> _consumerTags = new List<string>();

    public MQTask(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Start()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        // 假设我们有两个队列需要监听
        string[] queueNames = { "test_1", "test_2" };

        foreach (var queueName in queueNames)
        {
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            string consumerTag = _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            _consumers.Add(consumer);
            _consumerTags.Add(consumerTag);
        }
    }

    // @TODO: 这里有更多的任务需要处理的话 直接往里面加就好了
    private void Consumer_Received(object sender, BasicDeliverEventArgs e)
    {
        // 处理接收到的消息
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        Console.WriteLine($"Received message from queue: {e.RoutingKey}, Message: {message}");
        var message2 = Encoding.UTF8.GetString(e.Body.ToArray());
    }

    public void Stop()
    {
        foreach (var consumerTag in _consumerTags)
        {
            if (!string.IsNullOrEmpty(consumerTag) && _channel.IsOpen)
            {
                _channel.BasicCancel(consumerTag);
            }
        }

        if (_channel.IsOpen)
        {
            _channel.Close();
        }

        if (_connection.IsOpen)
        {
            _connection.Close();
        }
    }
}