# RabbitMQ Basic Setup and Usage

This guide will walk you through using C# console applications to set up and interact with RabbitMQ using Docker. RabbitMQ is a message broker that enables communication between different parts of an application.

## Prerequisites

Before you begin, make sure you have Docker installed on your system. You can download Docker from the official website: [Docker](https://www.docker.com/)

## 1. Start RabbitMQ Docker Container

Open a terminal and run the following command to start a RabbitMQ Docker container:

```bash
docker run -d --hostname rmq --name rabbit-server -p 8080:15672 -p 5672:5672 rabbitmq:3-management
```

Explanation of the command:

- `-d`: Run the container in detached mode.
- `--hostname rmq`: Set the hostname for the container as "rmq".
- `--name rabbit-server`: Assign the name "rabbit-server" to the container.
- `-p 8080:15672`: Map port 8080 on the host to RabbitMQ management interface's port 15672. This port is used to access the RabbitMQ management console.
- `-p 5672:5672`: Map port 5672 on the host to RabbitMQ's messaging port 5672. This port is used to send and receive messages.
- `rabbitmq:3-management`: Use the RabbitMQ Docker image with management interface (version 3).

## 2. Access RabbitMQ Management Console

Open a web browser and navigate to [http://localhost:8080](http://localhost:8080). This will take you to the RabbitMQ management console. You can log in using the default credentials:

- Username: `guest`
- Password: `guest`

## 3. Create Queues, Exchanges, and Bindings

In the RabbitMQ management console, you can create queues (message storage), exchanges (message routing), and bindings (connection between exchanges and queues).

- **Queues**: Queues hold the messages until they are consumed by consumers.
- **Exchanges**: Exchanges route messages to queues based on certain criteria.
- **Bindings**: Bindings connect exchanges to queues.

In this section we'll cover setting up the connection to the RabbitMQ Docker image using the ConnectionFactory, and creating queues, exchanges, and bindings using the IModel channel.
```csharp
// Create the connection
var factory = new ConnectionFactory
{
    Uri = new("amqp://guest:guest@localhost:5672"),
    ClientProvidedName = "Rabbit Client App"
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

// Declare the exchange
channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

// Declare the queue
channel.QueueDeclare(queueName, false, false, false, null);

// Bind the queue to the exchange
channel.QueueBind(queueName, exchangeName, routingKey, null);
```


## 4. Publish and Consume Messages

### Publishing Messages

You can publish messages to an exchange through the RabbitMQ management console or by using various programming languages and libraries.

In this section, we'll cover how to use the IModel channel to send messages to a RabbitMQ queue.

#### **`Client`**
```csharp
// ... Connection Setup ...

// Sending messages to the queue
for (int i = 0; i < 60; i++)
{
    string message = $"Message #{i + 1}";
    byte[] body = Encoding.UTF8.GetBytes(message);

    // Publish the message to the exchange
    channel.BasicPublish(exchangeName, routingKey, null, body);
}
```

### Consuming Messages

To consume messages, you need to write a consumer application using the RabbitMQ library of your chosen programming language. The consumer subscribes to a queue and processes messages as they arrive.

In this section, we'll cover how to use the EventingBasicConsumer to consume messages from a RabbitMQ queue.

#### **`Server 1`**
```csharp
// ... Connection setup code ...

// Receiving messages from the queue
var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (sender, args) =>
{
    var body = args.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);

    // Simulate processing time
    await Task.Delay(TimeSpan.FromSeconds(5));

    Console.WriteLine($"Message received: {message}");
    channel.BasicAck(args.DeliveryTag, false);
};

string consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.ReadKey();

channel.BasicCancel(consumerTag);
```

#### **`Server 2`**
```csharp
// ... Connection setup code ...

// Receiving messages from the queue
var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (sender, args) =>
{
    var body = args.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);

    // Simulate processing time
    await Task.Delay(TimeSpan.FromSeconds(3));

    Console.WriteLine($"Message received: {message}");
    channel.BasicAck(args.DeliveryTag, false);
};

string consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.ReadKey();

channel.BasicCancel(consumerTag);
```

### Closing Channel and Connection

In this section, we'll add the code to properly close the IModel channel and the connection to RabbitMQ after we are done sending or consuming messages.

```csharp
// ... Sending or Receiving messages code ...

// Close the channel and connection
channel.Close();
connection.Close();
```

## 5. Clean Up

When you're done experimenting with RabbitMQ, you can stop and remove the Docker container using the following commands:

```bash
docker stop rabbit-server
docker rm rabbit-server
```

This concludes the basic setup and usage guide for RabbitMQ with Docker.

### 6. Conclusion

Congratulations! You've successfully completed the basic setup and usage guide for RabbitMQ with Docker. By following this guide, you've gained insights into the fundamental concepts of message queuing, including queues, exchanges, bindings, and message publishing/consumption. You've also learned how to create a Dockerized RabbitMQ environment, access the management console, and interact with RabbitMQ using C# console applications.

Remember that this guide covers only the basics of RabbitMQ. As you become more familiar with RabbitMQ concepts, you can explore advanced features such as message acknowledgments, exchanges, routing, and more. For further learning, you can refer to the official RabbitMQ documentation: [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html).

RabbitMQ's capabilities provide a solid foundation for building scalable and reliable communication systems within your applications. You can now take the knowledge you've gained here and apply it to more complex scenarios, tailoring RabbitMQ to your specific use cases.

Thank you for using this guide, and we hope it serves as an introduction to your exciting journey with RabbitMQ. Happy messaging!