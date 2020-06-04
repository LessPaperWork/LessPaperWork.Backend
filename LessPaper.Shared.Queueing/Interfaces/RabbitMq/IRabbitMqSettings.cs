namespace LessPaper.Shared.Queueing.Interfaces.RabbitMq
{
    public interface IRabbitMqSettings
    {
        /// <summary>
        /// Hostname i.e localhost or 127.0.0.1
        /// </summary>
        string Host { get; }

        /// <summary>
        /// Username
        /// </summary>
        string Username { get;  }

        /// <summary>
        /// Password
        /// </summary>
        string Password { get;  }

        /// <summary>
        /// Unique server identity
        /// </summary>
        string ServerIdentity { get;  }
    }
}
