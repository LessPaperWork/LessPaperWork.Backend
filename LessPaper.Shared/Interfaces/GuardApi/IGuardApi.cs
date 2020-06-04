﻿using System;
using System.IO;
using System.Threading.Tasks;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Interfaces.General;
using LessPaper.Shared.Interfaces.GuardApi.Response;
using LessPaper.Shared.Interfaces.WriteApi.WriteObjectApi;

namespace LessPaper.Shared.Interfaces.GuardApi
{
    public interface IGuardApi
    {
        /// <summary>
        /// Register New User
        /// </summary>
        /// <param name="email"></param>
        /// <param name="passwordHash"></param>
        /// <param name="salt"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        Task<bool> RegisterNewUser(string email, string passwordHash, string salt, string userId);

        /// <summary>
        /// Get user credentials for comparison with given data.
        /// </summary>
        /// <param name="email">E-Mail address</param>
        /// <returns>Necessary credentials or null if user does not exist</returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        Task<IMinimalUserInformation> GetUserCredentials(string email);

        /// <summary>
        /// Get permissions of an array of files/directories
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="userId">User id</param>
        /// <param name="objectIds">Requested object ids</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        Task<IPermissionResponse[]> GetObjectsPermissions(string requestingUserId, string userId, string[] objectIds);

        /// <summary>
        /// Add a directory to a directory
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="parentDirectoryId">Parent directory id i.e. the root directory</param>
        /// <param name="directoryName">New directory name</param>
        /// <param name="newDirectoryId">Unique id of the directory</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        Task<bool> AddDirectory(string requestingUserId, string parentDirectoryId, string directoryName, string newDirectoryId);

        /// <summary>
        /// Deletes an file or an directory
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="objectId">File or directory id</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        Task<bool> DeleteObject(string requestingUserId, string objectId);

        /// <summary>
        /// Add a new file to a directory
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="directoryId">Directory Id</param>
        /// <param name="fileId">File id</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="fileSize">Size/Length of the file</param>
        /// <param name="encryptedKey">Encrypted key</param>
        /// <param name="documentLanguage">Language of the document</param>
        /// <param name="fileExtension">Type of the file</param>
        /// <param name="blobId">Id of the binary blob</param>
        /// <returns>Quick number</returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        Task<int> AddFile(
            string requestingUserId,
            string directoryId,
            string fileId,
            string blobId,
            string fileName,
            int fileSize, 
            string encryptedKey, 
            DocumentLanguage documentLanguage, 
            ExtensionType fileExtension);

        /// <summary>
        /// Update the metadata of an object i.e. the directory name
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="objectId">ObjectID of the Object where the metadata should be updated</param>
        /// <param name="updatedMetadata">New metadata</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        Task<bool> UpdateObjectMetadata(string requestingUserId, string objectId, IMetadataUpdate updatedMetadata);

        /// <summary>
        /// Retrieve metadata of an object
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="objectId">Directory or file id</param>
        /// <param name="revisionNumber">Version number. Newest file when not set</param>
        /// <returns>Returns metadata of directory or file</returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        /// <exception cref="FileNotFoundException"></exception>
        Task<IMetadata> GetMetadata(string requestingUserId, string objectId, uint? revisionNumber);

        /// <summary>
        /// Search for files and directories
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="directoryId">Search root directory</param>
        /// <param name="searchQuery">Search query</param>
        /// <param name="count">Limit of results</param>
        /// <param name="page">Result page</param>
        /// <returns>List of files and directories</returns>
        /// <exception cref="InvalidOperationException"></exception>
        Task<ISearchResult> Search(string requestingUserId, string directoryId, string searchQuery, uint count, uint page);
    }
}
