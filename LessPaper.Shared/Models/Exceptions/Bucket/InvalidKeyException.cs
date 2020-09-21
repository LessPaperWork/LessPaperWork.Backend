using System;
using System.Collections.Generic;
using System.Text;

namespace LessPaper.Shared.Models.Exceptions.Bucket
{
    public class InvalidKeyException : Exception
    {
        public InvalidKeyException(string message) : base(message)
        {
            
        }

        public InvalidKeyException(string message, Exception exception) : base(message, exception)
        {
            
        }
    }
}
