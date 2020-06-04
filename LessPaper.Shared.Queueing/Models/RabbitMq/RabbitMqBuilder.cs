using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreenPipes;
using LessPaper.Shared.Interfaces.Queuing;
using LessPaper.Shared.Queueing.Interfaces;
using LessPaper.Shared.Queueing.Interfaces.RabbitMq;
using MassTransit;

namespace LessPaper.Shared.Queueing.Models.RabbitMq
{
    /// <summary>
    /// Enables an easy access to the bus
    /// </summary>
    public class RabbitMqBuilder : IQueueBuilder
    {
        private readonly Dictionary<string, List<IConsumer>> subscribedQueues = new Dictionary<string, List<IConsumer>>();
        private readonly IRabbitMqSettings settings;

        
        /// <summary>
        /// Enables an easy access to the bus
        /// </summary>
        /// <param name="settings">Settings</param>
        public RabbitMqBuilder(IRabbitMqSettings settings)
        {
            this.settings = settings;
        }

        ///<inheritdoc/>
        public IQueueBuilder SubscribeSpecific<T>(string queue, Func<T, Task> callback) where T : class, new()
        {
            var proxy = new MassTransitConsumerProxy<T>(callback);
            if (!subscribedQueues.ContainsKey(queue))
                subscribedQueues.Add(queue, new List<IConsumer>());

            subscribedQueues[queue].Add(proxy);
            return this;
        }

        ///<inheritdoc/>
        public IQueueBuilder Subscribe<T>(Func<T, Task> callback) where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(settings.ServerIdentity))
                throw new Exception("Server identity not configured. Check the given configuration.");

            return SubscribeSpecific(settings.ServerIdentity, callback);
        }

        ///<inheritdoc/>
        public async Task<IQueueSender> Start()
        {
            if (string.IsNullOrWhiteSpace(settings.Host))
                throw new Exception("Host address not configured. Check the given configuration.");

            var busController = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                // ReSharper disable once StringLiteralTypo
                cfg.Host(new Uri($"rabbitmq://{settings.Host}"), host =>
                {
                    host.Username(settings.Username);
                    host.Password(settings.Password);
                });

                cfg.Durable = true;

                foreach (var subscribedQueue in subscribedQueues)
                {
                    var queueName = subscribedQueue.Key;
                    var registeredConsumers = subscribedQueue.Value;


                    cfg.ReceiveEndpoint(queueName, e =>
                    {
                        e.UseMessageRetry(r => r.Immediate(5));

                        foreach (var consumer in registeredConsumers)
                            e.Consumer(consumer.GetType(), x => consumer);
                        
                    });
                }
            });

            await busController.StartAsync();
            return new RabbitMqSender(busController, this, settings);
        }
    }
}
