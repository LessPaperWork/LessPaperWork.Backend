using System;
using System.Collections.Generic;
using System.Text;

namespace LessPaper.Shared.Interfaces.General
{
    public interface IBasicUserInformation
    {
        /// <summary>
        /// User id
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// Email Address
        /// </summary>
        string Email { get; }
        
        /// <summary>
        /// Public key
        /// </summary>
        string PublicKey { get; }
    }
}
