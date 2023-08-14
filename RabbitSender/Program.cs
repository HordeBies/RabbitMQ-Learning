using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Rabbit Client App";

IConnection cnn = factory.CreateConnection();

IModel channel = cnn.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);

for (int i = 0; i < 60; i++)
{
    Console.WriteLine($"Sending Message #{i+1}");
    string message = $"Message #{i+1}";
    byte[] body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchangeName, routingKey, null, body);
    await Task.Delay(TimeSpan.FromSeconds(1));
}

channel.Close();
cnn.Close();

Console.WriteLine("All messages sent!");