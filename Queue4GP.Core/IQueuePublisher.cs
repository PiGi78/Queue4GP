using System;

namespace Queue4GP.Core
{
    /// <summary>
    /// Publisher of messages into a queue
    /// </summary>
    /// <typeparam name="TMessage">Message type</typeparam>
    public interface IQueuePublisher<TMessage> : IDisposable
        where TMessage : IQueueMessage
    {

        /// <summary>
        /// Sends a message to the queue
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="ttl">Time To Live (0 = forever)</param>
        void Send(TMessage message, long ttl = 0);
    }
}