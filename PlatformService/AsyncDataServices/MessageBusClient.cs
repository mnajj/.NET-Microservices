using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
  public class MessageBusClient : IMessageBusClient
  {
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration configuration)
    {
      _configuration = configuration;
      var factory = new ConnectionFactory()
      {
        HostName = _configuration["RabbitMQHost"],
        Port = int.Parse(_configuration["RabbitMQPort"])
      };
      try
      {
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Couldn't connect to message bus: {ex.Message}");
      }
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
    }

    public void PublishNewPlatform(PlatformPublishDto platformPublishDto)
    {
      var message = JsonSerializer.Serialize(platformPublishDto);
      if (_connection.IsOpen)
      {
        SendMessage(message);
      }
      else
      {
        Console.WriteLine($"--> RabbitMQ Connection is Closed!");
      }
    }

    private void SendMessage(string message)
    {
      var body = Encoding.UTF8.GetBytes(message);
      _channel.BasicPublish(
        exchange: "trigger",
        routingKey: "",
        basicProperties: null,
        body: body);
    }

    public void Dispose()
    {
      Console.WriteLine($"--> Message Bus Disposed!");
      if (_channel.IsOpen)
      {
        _channel.Close();
        _connection.Close();
      }
    }
  }
}