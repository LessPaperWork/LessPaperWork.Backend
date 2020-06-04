using System;
using System.Collections.Generic;
using System.Text;

namespace LessPaper.Shared.Models.Exceptions
{
    public class UnexpectedBehaviourException : Exception
    {
        /// <inheritdoc />
        public UnexpectedBehaviourException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public UnexpectedBehaviourException(string message, Exception innerException) : base(message, innerException)
        {
        }
    
    }
}
