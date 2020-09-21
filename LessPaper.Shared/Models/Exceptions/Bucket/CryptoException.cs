using System;
using System.Collections.Generic;
using System.Text;

namespace LessPaper.Shared.Models.Exceptions.Bucket
{
    public class CryptoException : Exception
    {
        public CryptoException(string message) : base(message)
        {
            
        }

        public CryptoException(string message, Exception exception) : base(message, exception)
        {
            
        }

    }
}
