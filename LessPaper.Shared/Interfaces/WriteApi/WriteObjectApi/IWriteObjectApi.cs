using System;
using System.IO;
using System.Threading.Tasks;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Interfaces.General;

namespace LessPaper.Shared.Interfaces.WriteApi.WriteObjectApi
{
    public interface IWriteObjectApi
    {
        /// <summary>
        /// Add File as new Version
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="directoryId">Id of the parent directory</param>
        /// <param name="file">File</param>
        /// <param name="plaintextKey">Plaintext key used for encrypting the key</param>
        /// <param name="encryptedKey">Encrypted key used for saving in database</param>
        /// <param name="documentLanguage">Language of the file or all files in the directory</param>
        /// <param name="fileExtension">Type of the file</param>
        /// <returns>Upload Metadata</returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        Task<IUploadMetadata> UploadFile(string requestingUserId, string directoryId, Stream file, string plaintextKey,
            string encryptedKey, DocumentLanguage documentLanguage, ExtensionType fileExtension);

        /// <summary>
        /// Create Directory
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="directoryId">Id of parent directory</param>
        /// <param name="subDirectoryName">Name of new Directory</param>
        /// <returns>Directory Metadata</returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        Task<IDirectoryMetadata> CreateDirectory(string requestingUserId, string directoryId, string subDirectoryName);

        /// <summary>
        /// Update Metadata of file or directory 
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="objectId">Object ID</param>
        /// <param name="metadataUpdate"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        Task<bool> UpdateMetadata(string requestingUserId, string objectId, IMetadataUpdate metadataUpdate);

        /// <summary>
        /// Flags Object as deleted
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="objectId"></param>
        /// <returns>Return true when file is deleted</returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        Task<bool> DeleteObject(string requestingUserId, string objectId);
    }
}
