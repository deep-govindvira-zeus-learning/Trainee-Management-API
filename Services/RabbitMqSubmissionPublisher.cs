using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using TraineeManagementApi.DTOs;

public interface ISubmissionPublisher
{
    Task<bool> Publish(SubmissionProcessingRequested message);
}

public class RabbitMqSubmissionPublisher : ISubmissionPublisher
{
    private readonly ILogger<RabbitMqSubmissionPublisher> _logger;
    private IConnection? _connection;
    private RabbitMQ.Client.IChannel? _channel;
    private readonly string _queueName;
    private readonly IConfiguration _configuration;

    public RabbitMqSubmissionPublisher(ILogger<RabbitMqSubmissionPublisher> logger, IConfiguration configuration)
    {
        _logger = logger;
        _queueName = configuration["RabbitMq:QueueName"] ?? "submission-processing";
        _configuration = configuration;
        InitializeRabbitMq();
    }

    private async Task InitializeRabbitMq()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"] ?? "localhost",

                // Convert the string to an int, defaulting to 5672 if missing or invalid
                Port = int.TryParse(_configuration["RabbitMq:Port"], out var port) ? port : 5672,

                UserName = _configuration["RabbitMq:Username"] ?? "guest",
                Password = _configuration["RabbitMq:Password"] ?? "guest",
                VirtualHost = _configuration["RabbitMq:VirtualHost"] ?? "/"
            };


            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            // Declare a durable queue
            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Could not initialize connection to RabbitMQ.");
        }
    }

    public async Task<bool> Publish(SubmissionProcessingRequested message)
    {
        if (_channel == null || _channel.IsOpen == false)
        {
            _logger.LogError("RabbitMQ channel is unavailable. Cannot publish MessageId: {MessageId}", message.MessageId);
            return false;
        }

        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true, // Replaces DeliveryMode = 2
                ContentType = "application/json",
                CorrelationId = Guid.NewGuid().ToString()
            };

            properties.Persistent = true; // Message survives broker restart
            properties.MessageId = message.MessageId.ToString();
            properties.CorrelationId = message.CorrelationId.ToString();

            await _channel.BasicPublishAsync(
                exchange: string.Empty, // Default exchange
                routingKey: _queueName,
                mandatory: false, // You must include this boolean argument
                basicProperties: properties,
                body: body
            );

            _logger.LogInformation("Successfully published message to RabbitMQ. MsgId: {MessageId}, CorrelationId: {CorrelationId}",
                message.MessageId, message.CorrelationId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message due to a broker exception. MsgId: {MessageId}", message.MessageId);
            return false;
        }
    }

    public async Task Dispose()
    {
        await _channel?.CloseAsync();
        await _connection?.CloseAsync();
    }
}
