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

The library can be used on ASP.NET core also.
You simply need to set the connection info on the appsettings.json file:

```
  "Queue4GP": {
    "Host": "localhost",
    "Port": 5672,
    "User": "guest",
    "Password":  "guest"
  }
```

Then you have to register this configuration on the standard IOC:

```
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Configure the queue settings
            services.Configure<QueueConfiguration>(options => Configuration.GetSection("Queue4GP").Bind(options));
        }
```

Finally, in the controller you want to send the message, you need to:
- Add the IOption<QueueConfiguration> for read the configuration from the appsettings.json file
- Create the publisher
- Send the message

In this example you can see how to send a MyMessage to the queue:

```
    [Route("api/[controller]")]
    [ApiController]
    public class QueueController : ControllerBase
    {

        public QueueController(IOptions<QueueConfiguration> config)
        {
            QueueConfiguration = config.Value;
        }


        private QueueConfiguration QueueConfiguration { get; }


        // GET api/values
        [HttpGet("SendMessage")]
        public ActionResult SendMessage([FromQuery]string message)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrEmpty(message)) return BadRequest("message is mandatory");

            using (var publisher = new MyPublisher(QueueConfiguration))
            {
                var msg = new MyMessage
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Text = message
                };
                publisher.Send(msg);
            }

            return Ok();
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
```

### Time To Live

Sometimes a message has to be delivered in a certain amount of time or could be not delivered.
This amount of time is usually called Time To Live (TTL).

If you want to define a TTL for the message, you can simply pass it to the Send method:

```

            using (var publisher = new MyPublisher(QueueConfiguration))
            {
                var msg = new MyMessage
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Text = message
                };
                /* 
                 * The message will stay in the queue for 5 seconds. 
                 * If noone listens for it in 5 seconds, nobody will receive the message
                 */
                long ttl = 5000L;

                publisher.Send(msg, ttl);
            }

```


## Built With

* [RabbitMQ.Client](https://www.rabbitmq.com/dotnet-api-guide.html) - Used for comunication with the RabbitMQ server
* [Jon.NET (Newtonsoft)](https://www.newtonsoft.com/json) - Used to serialize/deserialize queue messages

