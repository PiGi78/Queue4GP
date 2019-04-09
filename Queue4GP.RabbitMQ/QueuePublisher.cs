using System;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Queue4GP.Core;
using RabbitMQ.Client;

namespace Queue4GP.RabbitMQ
{

    /// <summary>
    /// Queue publisher
    /// </summary>
    /// <typeparam name="TMessage">Message type</typeparam>
    public class QueuePublisher<TMessage> : IQueuePublisher<TMessage>
        where TMessage : IQueueMessage
    {


        /// <summary>
        /// Creates a new instance of <see cref="QueuePublisher{TMessage}"/>
        /// </summary>
        /// <param name="configuration">Configurations to use</param>
        public QueuePublisher(QueueConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        /// <summary>
        /// Creates a new instance of <see cref="QueuePublisher{TMessage}"/>
        /// </summary>
        /// <param name="configuration">IOptionsSnapshot wher to find the configuration object</param>
        public QueuePublisher(IOptionsSnapshot<QueueConfiguration> configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            Configuration = configuration.Value;
            if (Configuration == null) throw new ArgumentException("Configuration.Value cannot be null", nameof(configuration));
        }

        /// <summary>
        /// Queue configuration
        /// </summary>
        private QueueConfiguration Configuration { get; }


        /// <summary>
        /// Send a message to the queue
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="ttl">Time To Live (0 = forever)</param>
        public void Send(TMessage message, long ttl = 0)
        {
            if (ExchangeInfo == null)
            {
                ExchangeInfo = RabbitHelper.CreateExchange(Configuration);
            }
            var routingKey = typeof(TMessage).Name;
            IBasicProperties props = ExchangeInfo.Channel.CreateBasicProperties();
            props.ContentEncoding = "UTF-8";
            props.ContentType = "application/json";
            props.DeliveryMode = 2;
            if (ttl > 0)
            {
                props.Expiration = ttl.ToString();
            }
            ExchangeInfo.Channel.BasicPublish(exchange: ExchangeInfo.ExchangeName,
                                 routingKey: routingKey,
                                 body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)),
                                 basicProperties: props);
        }


        /// <summary>
        /// Exchange info
        /// </summary>
        private RabbitExchangeInfo ExchangeInfo { get; set; }


        #region Disposable

        private bool isDisposed = false;

        /// <summary>
        /// Releases the resources
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) throw new ObjectDisposedException(GetType().Name);
            ExchangeInfo?.Channel?.Dispose();
            ExchangeInfo?.Connection?.Dispose();
        }

        #endregion
    }
}