namespace Queue4GP.Core
{
    /// <summary>
    /// Message of a queue
    /// </summary>
    public interface IQueueMessage
    {

        /// <summary>
        /// Message id
        /// </summary>
        string MessageId { get; set; }

    }
}