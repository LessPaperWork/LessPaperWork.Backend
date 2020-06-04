using System;

namespace LessPaper.Shared.Models.Exceptions
{
    public class ObjectNotResolvableException : Exception
    {
        /// <inheritdoc />
        public ObjectNotResolvableException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public ObjectNotResolvableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
