using System;
using System.Threading;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.Queuing;
using LessPaper.Shared.Queueing.Interfaces;
using LessPaper.Shared.Queueing.Interfaces.RabbitMq;
using MassTransit;

namespace LessPaper.Shared.Queueing.Models.RabbitMq
{
    /// <summary>
    /// Enables sending of messages
    /// Use RabbitMqBuilder as a factory to create an instance.
    /// </summary>
    public class RabbitMqSender : IQueueSender
    {
        private readonly IBusControl busController;
        private readonly IQueueBuilder queueBuilder;
        private readonly IRabbitMqSettings settings;

        /// <summary>
        /// Enables sending of messages
        /// Use RabbitMqBuilder as a factory to create an instance.
        /// </summary>
        /// <param name="busController">MassTransit bus-controller</param>
        /// <param name="queueBuilder">Latest queue builder</param>
        /// <param name="settings">RabbitMq settings</param>
        internal RabbitMqSender(IBusControl busController, IQueueBuilder queueBuilder, IRabbitMqSettings settings)
        {
            this.busController = busController;
            this.queueBuilder = queueBuilder;
            this.settings = settings;
        }

        ///<inheritdoc/>
        public async Task<IQueueBuilder> Stop()
        {
            await busController.StopAsync();
            return queueBuilder;
        }

        ///<inheritdoc/>
        public async Task SendToSpecific<T>(string queueName, T sendObj) where T : class
        {
            var sendEndpointTask = await busController.GetSendEndpoint(new Uri(string.Concat("rabbitmq://", settings.Host, "/", queueName)));
            await sendEndpointTask.Send(sendObj);
        }

        ///<inheritdoc/>
        public async Task Send<T>(T sendObj) where T : class
        {
            await busController.Publish(sendObj);
        }
    }
}