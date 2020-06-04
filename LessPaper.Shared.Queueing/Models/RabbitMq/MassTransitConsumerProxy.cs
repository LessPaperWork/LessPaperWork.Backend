using System;
using System.Threading.Tasks;
using MassTransit;

namespace LessPaper.Shared.Queueing.Models.RabbitMq
{
    /// <summary>
    /// Imitates a consumer for a specific type
    /// </summary>
    /// <typeparam name="T">Expected message type</typeparam>
    internal class MassTransitConsumerProxy<T> : IConsumer<T> where T : class, new()
    {
        private readonly Func<T, Task> callback;

        /// <summary>
        /// Imitates a consumer for a specific type
        /// </summary>
        /// <param name="callback">Callback which is called when a new message arrives</param>
        public MassTransitConsumerProxy(Func<T, Task> callback)
        {
            this.callback = callback;
        }

        ///<inheritdoc/>
        public async Task Consume(ConsumeContext<T> context)
        {
            await callback.Invoke(context.Message);
        }
    }
}