using System;
using System.Threading.Tasks;

namespace LessPaper.Shared.Interfaces.Queuing
{
    public interface IQueueBuilder
    {
        /// <summary>
        /// Subscribe to a specific queue
        /// </summary>
        /// <typeparam name="T">Expected object type</typeparam>
        /// <param name="queue">Target queue</param>
        /// <param name="callback">Callback</param>
        /// <returns></returns>
        IQueueBuilder SubscribeSpecific<T>(string queue, Func<T, Task> callback) where T: class, new();

        /// <summary>
        /// Subscribe to server queue
        /// </summary>
        /// <typeparam name="T">Expected object type</typeparam>
        /// <param name="callback">Callback</param>
        /// <returns></returns>
        IQueueBuilder Subscribe<T>(Func<T, Task> callback) where T : class, new();

        /// <summary>
        /// Start the listener
        /// </summary>
        /// <returns></returns>
        Task<IQueueSender> Start();
    }
}
