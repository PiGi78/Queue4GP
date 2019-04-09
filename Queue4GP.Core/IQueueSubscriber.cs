using System;

namespace Queue4GP.Core
{
    /// <summary>
    /// Queue subscriber
    /// </summary>
    /// <typeparam name="TMessage">Message type</typeparam>
    public interface IQueueSubscriber<TMessage> : IDisposable
        where TMessage : IQueueMessage
    {


        /// <summary>
        /// Handler for new messages
        /// </summary>
        IQueueHandler<TMessage> QueueHandler { get; }


        /// <summary>
        /// Start listening for new messages
        /// </summary>
        void StartListening();


        /// <summary>
        /// Stop listening for new messages
        /// </summary>
        void StopListening();
    }
}