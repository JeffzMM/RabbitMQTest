using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
{
    private static readonly string HostName = "localhost";
    private static readonly string QueueName = "testQueue";

    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = HostName };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: QueueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        string message = "Hello RabbitMQ!";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "",
                             routingKey: QueueName,
                             basicProperties: null,
                             body: body);

        Console.WriteLine(" [x] Sent {0}", message);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var receivedMessage = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", receivedMessage);
        };

        channel.BasicConsume(queue: QueueName,
                             autoAck: true,
                             consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
