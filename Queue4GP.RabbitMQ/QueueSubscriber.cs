using System;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Queue4GP.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Queue4GP.RabbitMQ
{

    /// <summary>
    /// Queue subscriber
    /// </summary>
    /// <typeparam name="TMessage">Type of param</typeparam>
    public class QueueSubscriber<TMessage> : IDisposable, IQueueSubscriber<TMessage>
        where TMessage : IQueueMessage
    {

        /// <summary>
        /// Creates a new instance of <see cref="QueueSubscriber{TMessage}"/>
        /// </summary>
        /// <param name="configuration">Configurations</param>
        /// <param name="handler">Handler of messages</param>
        public QueueSubscriber(QueueConfiguration configuration, IQueueHandler<TMessage> handler)
        {
            QueueHandler = handler ?? throw new ArgumentNullException(nameof(handler));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        /// <summary>
        /// Creates a new instance of <see cref="QueueSubscriber{TMessage}"/>
        /// </summary>
        /// <param name="configuration">IOptionsSnapshot where find the configuration</param>
        /// <param name="handler">Handler of messages</param>
        public QueueSubscriber(IOptionsSnapshot<QueueConfiguration> configuration, IQueueHandler<TMessage> handler)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            QueueHandler = handler ?? throw new ArgumentNullException(nameof(handler));
            Configuration = configuration.Value;
            if (Configuration == null) throw new ArgumentException("Configuration.Value cannot be null", nameof(configuration));
        }


        #region Private fields

        /// <summary>
        /// Queue configuration
        /// </summary>
        private QueueConfiguration Configuration { get; }


        /// <summary>
        /// Exchange information
        /// </summary>
        private RabbitExchangeInfo ExchangeInfo { get; set; }


        /// <summary>
        /// Event consumer
        /// </summary>
        private EventingBasicConsumer Consumer { get; set; }


        #endregion


        public IQueueHandler<TMessage> QueueHandler { get; }

        /// <summary>
        /// Starts listening for messages
        /// </summary>
        public void StartListening()
        {
            // Queue name
            var queueName = QueueHandler.GetType().FullName;
            // Creates the exchange and queue
            if (ExchangeInfo == null)
            {
                ExchangeInfo = RabbitHelper.CreateExchange(Configuration);
                // Coda
                ExchangeInfo.Channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
                // Routing key
                var routingKey = typeof(TMessage).Name;
                // Binding
                ExchangeInfo.Channel.QueueBind(queueName, ExchangeInfo.ExchangeName, routingKey);
            };
            // Create a consumer
            Consumer = new EventingBasicConsumer(ExchangeInfo.Channel);
            Consumer.Received += (model, arg) => {
                var json = Encoding.UTF8.GetString(arg.Body);
                var obj = JsonConvert.DeserializeObject<TMessage>(json);
                QueueHandler.HandleMessage(obj);
                ExchangeInfo.Channel.BasicAck(deliveryTag: arg.DeliveryTag, multiple: false);
            };
            ExchangeInfo.Channel.BasicConsume(queueName, autoAck: false, consumer: Consumer);
        }


        /// <summary>
        /// Stop listening for messages
        /// </summary>
        public void StopListening()
        {
            if (Consumer == null) throw new InvalidOperationException("Start listening before stop it");
            CloseChannel();
        }


        private void CloseChannel()
        {
            if (Consumer != null)
            {
                ExchangeInfo.Channel.BasicCancel(Consumer.ConsumerTag);
            }
            ExchangeInfo?.Channel?.Dispose();
            ExchangeInfo?.Connection?.Dispose();
            ExchangeInfo = null;
            Consumer = null;
        }


        #region Disposable

        private bool isDisposed = false;

        /// <summary>
        /// Releases the resources
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) throw new ObjectDisposedException(GetType().Name);
            CloseChannel();
        }

        #endregion


    }
}