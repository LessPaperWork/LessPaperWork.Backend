using System;

namespace LessPaper.Shared.Models.Exceptions
{
    public class DatabaseException : Exception
    {
        /// <inheritdoc />
        public DatabaseException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public DatabaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
