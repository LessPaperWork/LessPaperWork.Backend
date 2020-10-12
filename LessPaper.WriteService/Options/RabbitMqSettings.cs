using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes.Internals.Mapping;
using LessPaper.Shared.Queueing.Interfaces.RabbitMq;
using Microsoft.Extensions.Options;

namespace LessPaper.WriteService.Options
{
    public class RabbitMqSettings : IRabbitMqSettings
    {
        // ReSharper disable once UnusedMember.Global
        public RabbitMqSettings()
        {
        }

        public RabbitMqSettings(IOptions<AppSettings> settings)
        {
            Host = settings.Value.RabbitMq.Host;
            Username = settings.Value.RabbitMq.Username;
            Password = settings.Value.RabbitMq.Password;
            ServerIdentity = settings.Value.RabbitMq.ServerIdentity;
        }
        
        /// <inheritdoc />
        public string Host { get; set; }

        /// <inheritdoc />
        public string Username { get; set; }

        /// <inheritdoc />
        public string Password { get; set; }

        /// <inheritdoc />
        public string ServerIdentity { get; set; }
    }
}
