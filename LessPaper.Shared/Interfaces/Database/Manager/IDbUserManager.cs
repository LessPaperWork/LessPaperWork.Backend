using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Interfaces.General;

namespace LessPaper.Shared.Interfaces.Database.Manager
{
    public interface IDbUserManager
    {
        /// <summary>
        /// Add a new user to the database
        /// </summary>
        /// <param name="userId">New user id</param>
        /// <param name="rootDirectoryId">New id of the root directory (will be created)</param>
        /// <param name="email">Email address of the user</param>
        /// <param name="hashedPassword">Hashed password</param>
        /// <param name="salt">Salt used to generate the hash</param>
        /// <param name="publicKey">Fresh public key</param>
        /// <param name="encryptedPrivateKey">Fresh encrypted private key</param>
        /// <returns></returns>
        Task<bool> InsertUser(string userId, string rootDirectoryId, string email, string hashedPassword, string salt, string publicKey, string encryptedPrivateKey);

        /// <summary>
        /// Delete user including all directories, files and every other related information
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="userId"></param>
        /// <returns>Blob ids</returns>
        Task<string[]> DeleteUser(string requestingUserId, string userId);

        /// <summary>
        /// Get public user information 
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="userId">Target user</param>
        /// <returns></returns>
        Task<IBasicUserInformation> GetBasicUserInformation(string requestingUserId, string userId);

        /// <summary>
        /// Get private user information 
        /// </summary>
        /// <param name="email">Id of the requesting user</param>
        /// <returns></returns>
        Task<IExtendedUserInformation> GetUserInformation(string email);
    }
}
