using LessPaper.Shared.Queueing.Interfaces;
using LessPaper.Shared.Queueing.Interfaces.RabbitMq;

namespace LessPaper.Shared.Queueing.Models.RabbitMq
{
    /// <summary>
    /// Settings to access the queue
    /// </summary>
    public class RabbitMqSettings : IRabbitMqSettings
    {
        /// <summary>
        /// Settings to access the queue
        /// </summary>
        /// <param name="host">Hostname</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="serverIdentity">Unique server identity</param>
        public RabbitMqSettings(string host, string username, string password, string serverIdentity)
        {
            Host = host;
            Username = username;
            Password = password;
            ServerIdentity = serverIdentity;
        }

        /// <summary>
        /// Settings to access the queue
        /// </summary>
        public RabbitMqSettings()
        {
        }

        ///<inheritdoc/>
        public string Host { get; set; }

        ///<inheritdoc/>
        public string Username { get; set; }

        ///<inheritdoc/>
        public string Password { get; set; }

        ///<inheritdoc/>
        public string ServerIdentity { get; set; }
    }
}
