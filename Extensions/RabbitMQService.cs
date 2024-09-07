using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace user_service_api.Extensions;
public interface IRabbitMQService
{
    Task SendAsync(string queueName, string message);
    void Send(string queueName, string message);
    void Receive(string queueName, Action<string> messageHandler);
    Task ReceiveAsync(string queueName, Func<string, Task> messageHandler);
    void Dispose();
}
public class RabbitMQService:IRabbitMQService
{ 
    private readonly IConnection _connection;
    private readonly IModel _channel;
    
    public RabbitMQService(string hostname)
    {
        var factory = new ConnectionFactory() { HostName = hostname };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public async Task SendAsync(string queueName, string message)
    {
        _channel.QueueDeclare(queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        await Task.Run(() => _channel.BasicPublish(exchange: "",
            routingKey: queueName,
            basicProperties: null,
            body: body));
    }
    public void Send(string queueName, string message)
    {
        _channel.QueueDeclare(queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "",
            routingKey: queueName,
            basicProperties: null,
            body: body);
    }

    public void Receive(string queueName, Action<string> messageHandler)
    {
        _channel.QueueDeclare(queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            messageHandler?.Invoke(message); // 处理接收到的消息
        };

        _channel.BasicConsume(queue: queueName,
            autoAck: true,
            consumer: consumer);
    }

    public async Task ReceiveAsync(string queueName, Func<string, Task> messageHandler)
    {
        _channel.QueueDeclare(queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            try
            {
                await messageHandler(message);
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation if needed
            }
        };

        _channel.BasicConsume(queue: queueName,
            autoAck: true,
            consumer: consumer);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}