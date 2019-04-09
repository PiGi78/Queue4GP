using RabbitMQ.Client;

namespace Queue4GP.RabbitMQ
{
    /// <summary>
    /// Info about rabbit exchange
    /// </summary>
    class RabbitExchangeInfo
    {

        /// <summary>
        /// Connection
        /// </summary>
        public IConnection Connection { get; set; }


        /// <summary>
        /// Queue channel
        /// </summary>
        public IModel Channel { get; set; }
        

        /// <summary>
        /// Name of the exchange
        /// </summary>
        public string ExchangeName { get; set; }

    }
}