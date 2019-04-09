namespace Queue4GP.Core
{
    /// <summary>
    /// Handler of messages of a queue
    /// </summary>
    /// <typeparam name="TMessage">Type of the message</typeparam>
    public interface IQueueHandler<TMessage>
        where TMessage : IQueueMessage
    {


        /// <summary>
        /// Handle a new message on the queue
        /// </summary>
        /// <param name="message">Message arrived</param>
        void HandleMessage(TMessage message);
    }
}