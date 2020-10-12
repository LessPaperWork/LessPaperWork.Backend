using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using LessPaper.Shared.Rest.Models.Dtos;

namespace LessPaper.Shared.Rest.Exceptions
{
    /// <summary>
    /// Indicated an invalid status code
    /// </summary>
    public class InvalidStatusCodeException : Exception
    {
        /// <summary>
        /// Server message
        /// </summary>
        public MessageDto ServerMessage { get; }

        /// <summary>
        /// Indicated an invalid status code
        /// </summary>
        /// <param name="expected">Expected status code</param>
        /// <param name="current">Current status code</param>
        public InvalidStatusCodeException(HttpStatusCode expected, HttpStatusCode current) :
            base("Server returned an unexpected status code. " +
                 $"Expected was code {(int)expected} but client received {(int)current}")
        {
        }

        /// <summary>
        /// Indicated an invalid status code
        /// </summary>
        /// <param name="expected">Expected status code</param>
        /// <param name="current">Current status code</param>
        /// <param name="serverMessage">Message that the server delivers</param>
        public InvalidStatusCodeException(HttpStatusCode expected, HttpStatusCode current, MessageDto serverMessage) :
            base(serverMessage != null ? "Server returned an unexpected status code. " +
                 $"Expected was code {(int)expected} but client received {(int)current}. Server responded with message: {serverMessage.Message}" : 
                $"Server returned an unexpected status code. Expected was code {(int)expected} but client received {(int)current}")
        {
            ServerMessage = serverMessage;
        }
    }
}
