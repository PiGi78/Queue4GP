using System;
using RabbitMQ.Client;

namespace Queue4GP.RabbitMQ
{

    /// <summary>
    /// Helper for Rabbit MQ
    /// </summary>
    class RabbitHelper
    {

        /// <summary>
        /// Creates a RabbitMQ exchange
        /// </summary>
        /// <param name="configuration">Queue configuration</param>
        /// <returns>Informations about the created exchange</returns>
        public static RabbitExchangeInfo CreateExchange(QueueConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            // Exchange name
            var exchangeName = "Queue4GP";
            // Topic type
            var exchangeType = "topic";
            // Connection
            var factory = new ConnectionFactory
            {
                HostName = configuration.Host,
                Port = configuration.Port,
                UserName = configuration.User,
                Password = configuration.Password
            };
            var connection = factory.CreateConnection();
            // Channel
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType, durable: true, autoDelete: false);

            // Returns the result
            return new RabbitExchangeInfo
            {
                Connection = connection,
                ExchangeName = exchangeName,
                Channel = channel
            };
        }
    }
}