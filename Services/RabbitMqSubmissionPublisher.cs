using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using TraineeManagementApi.DTOs;

public interface ISubmissionPublisher
{
    Task<bool> Publish(SubmissionProcessingRequested message);
}

public class RabbitMqSubmissionPublisher : ISubmissionPublisher, IAsyncDisposable
{
    private readonly ILogger<RabbitMqSubmissionPublisher> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _queueName;

    private IConnection? _connection;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public RabbitMqSubmissionPublisher(ILogger<RabbitMqSubmissionPublisher> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _queueName = configuration["RabbitMq:QueueName"] ?? "submission-processing";
        // REMOVED: Do not call async methods in constructor. Let it initialize lazily on demand.
    }

    /// <summary>
    /// Thread-safe, asynchronous connection initializer. Guaranteed to run completely before any message is published.
    /// </summary>
    private async Task<IConnection> GetConnectionAsync()
    {
        if (_connection != null && _connection.IsOpen)
        {
            return _connection;
        }

        await _connectionLock.WaitAsync();
        try
        {
            // Double-check pattern after acquiring lock
            if (_connection != null && _connection.IsOpen)
            {
                return _connection;
            }

            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"] ?? "localhost",
                Port = int.TryParse(_configuration["RabbitMq:Port"], out var port) ? port : 5672,
                UserName = _configuration["RabbitMq:Username"] ?? "guest",
                Password = _configuration["RabbitMq:Password"] ?? "guest",
                VirtualHost = _configuration["RabbitMq:VirtualHost"] ?? "/",
                AutomaticRecoveryEnabled = true, // Essential for handling dropped server connections
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _logger.LogInformation("Publisher establishing connection to RabbitMQ broker...");
            _connection = await factory.CreateConnectionAsync();
            return _connection;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Could not initialize connection to RabbitMQ.");
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task<bool> Publish(SubmissionProcessingRequested message)
    {
        try
        {
            // 1. Ensure connection is up and fully awaited
            var connection = await GetConnectionAsync();

            // 2. Open a transient channel scoped entirely to this specific message delivery
            await using var channel = await connection.CreateChannelAsync();

            var queueArgs = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", "submission.retry.exchange" },
                { "x-dead-letter-routing-key", "submission.retry.key" }
            };


            // 3. Declare topology safely to ensure queue existence
            await channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs
            );

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true, // Survives RabbitMQ server crashes/restarts
                ContentType = "application/json",
                MessageId = message.MessageId.ToString(),
                CorrelationId = message.CorrelationId.ToString()
            };

            // Publish through your central exchange routing key
            await channel.BasicPublishAsync(
                exchange: "submission.main.exchange",
                routingKey: "submission.process.key",
                mandatory: false,
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

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing connection during publisher disposal.");
        }
        finally
        {
            _connectionLock.Dispose();
        }
    }
}
