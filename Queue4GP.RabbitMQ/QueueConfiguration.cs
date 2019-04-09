namespace Queue4GP.RabbitMQ
{    
    
    /// <summary>
    /// Configuration of the queue
    /// </summary>
    public class QueueConfiguration
    {

        /// <summary>
        /// Host of the queue server
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port of the queue server
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string User { get; set; }


        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
}