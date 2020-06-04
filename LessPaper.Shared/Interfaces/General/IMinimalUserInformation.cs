using System;
using System.Collections.Generic;
using System.Text;

namespace LessPaper.Shared.Interfaces.General
{
    public interface IMinimalUserInformation
    {
        /// <summary>
        /// Email Address
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Hashed version of the password + salt
        /// </summary>
        string PasswordHash { get; }

        /// <summary>
        /// Used salt
        /// </summary>
        string Salt { get; }

        /// <summary>
        /// Id of the root directory
        /// </summary>
        string RootDirectoryId { get; }

        /// <summary>
        /// Encrypted private key
        /// </summary>
        string EncryptedPrivateKey { get; }
    }
}
