# Queue4GP

This library is useful when you need to manage a simple queue.
It is not intended for anybody needs a full queue management.

Right now it works only with [RabbitMQ](https://www.rabbitmq.com/).

## NuGet

To install the package run the following command on the Package Manager Console:

```
PM> Install-Package Queue4GP.RabbitMQ
```

## Prerequisites

A RabbitMQ server is required.
The fastest way to configure it is to use a Docker container:

```
docker run -d --hostname my-rabbit --name my-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management
```

This command will install a new docker container named my-rabbit that uses the following port:
- 5672: used by the library for send/recive messages
- 15672: used for web management. Browe to http://localhost:15672 for see the RabbitMQ management website (guest/guest are the default credentials)


## Use the library

### Console application

The library can be used in a console application. In this sample you can see how much easy is to use the library.

```
    class Program
    {
        static void Main(string[] args)
        {
            // Rabbit MQ configuration
            var config = new QueueConfiguration
            {
                Host = "localhost",
                Port = 5672,
                User = "guest",
                Password = "guest"
            };

            // Istantiate a subscriber
            using (var subscriber = new QueueSubscriber<MyMessage>(config, new MyHandler()))
            {
                // Starts listening
                subscriber.StartListening();

                // Istantiate a subscriber
                using (var publisher = new MyPublisher(config))
                {
                    // Running while put an empty message
                    while (true)
                    {
                        Console.WriteLine("Write message text (empty for exit)");
                        // Creates a message
                        var msg = new MyMessage { MessageId = DateTime.Now.ToLongDateString() };
                        msg.Text = Console.ReadLine();
                        if (string.IsNullOrEmpty(msg.Text)) break;

                        // Publish it to the queue (and the subscriber will write it on the console)
                        publisher.Send(msg);
                    }
                }
                // Stop listening
                subscriber.StopListening();
            }
        }
    }


    /// <summary>
    /// Message to send/receive
    /// </summary>
    class MyMessage : IQueueMessage
    {

        /// <summary>
        /// Message id
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Message text
        /// </summary>
        public string Text { get; set; }
    }


    /// <summary>
    /// Subscriber handler. Will be called any time a message arrives to the queue
    /// </summary>
    class MyHandler : IQueueHandler<MyMessage>
    {
        public void HandleMessage(MyMessage message)
        {
            // Handles the message
            Console.WriteLine($"R: {message.MessageId} - {message.Text}");
        }
    }


    /// <summary>
    /// Publisher: sends messages to the queue
    /// </summary>
    class MyPublisher : QueuePublisher<MyMessage>
    {
        public MyPublisher(QueueConfiguration configuration) : base(configuration)
        {
        }
    }
```

### ASP.NET Core




## Built With

* [RabbitMQ.Client](https://www.rabbitmq.com/dotnet-api-guide.html) - Used for comunication with the RabbitMQ server
* [Jon.NET (Newtonsoft)](https://www.newtonsoft.com/json) - Used to serialize/deserialize queue messages

