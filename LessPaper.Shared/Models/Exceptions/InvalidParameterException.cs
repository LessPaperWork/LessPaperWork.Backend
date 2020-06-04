using System;
using System.Collections.Generic;
using System.Text;

namespace LessPaper.Shared.Models.Exceptions
{
    public class InvalidParameterException : Exception
    {
        /// <inheritdoc />
        public InvalidParameterException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public InvalidParameterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
