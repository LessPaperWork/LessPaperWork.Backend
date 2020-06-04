using System.Threading.Tasks;

namespace LessPaper.Shared.Interfaces.Queuing
{
    /// <summary>
    /// Enables sending of messages to the queueing system
    /// </summary>
    public interface IQueueSender
    {
        /// <summary>
        /// Stop the listener
        /// </summary>
        /// <returns></returns>
        Task<IQueueBuilder> Stop();

        /// <summary>
        /// Send to specific queue. This may results in a skipped queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue">Queue name</param>
        /// <param name="sendObj">Object to send. Make sure the receiver uses the same object. Which means it must have the same namespace and classname</param>
        /// <returns></returns>
        Task SendToSpecific<T>(string queue, T sendObj) where T : class;

        /// <summary>
        /// Send to every listener. Only the first listener who has nothing to do gets the message. First-Come-First-Serve.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sendObj">Object to send. Make sure the receiver uses the same object. Which means it must have the same namespace and classname</param>
        /// <returns></returns>
        Task Send<T>(T sendObj) where T : class;
    }
}