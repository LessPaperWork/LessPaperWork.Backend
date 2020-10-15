using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Interfaces.General;
using LessPaper.Shared.Interfaces.GuardApi.Response;

namespace LessPaper.Shared.Interfaces.Database.Manager
{
    public interface IDbFileManager : IDbObjectOperations
    {
        /// <summary>
        /// Add a new file to a directory
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="directoryId">Directory Id</param>
        /// <param name="fileId">File id</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="fileSize">Size/Length of the file</param>
        /// <param name="encryptedKey">Encrypted keys userid -> encrypted key</param>
        /// <param name="documentLanguage">Language of the document</param>
        /// <param name="fileExtension">Type of the file</param>
        /// <param name="blobId">Id of the binary blob</param>
        /// <returns>Quick number</returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        Task<uint> InsertFile(
            string requestingUserId,
            string directoryId,
            string fileId,
            string blobId,
            string fileName,
            int fileSize,
            Dictionary<string, string> encryptedKey,
            DocumentLanguage documentLanguage,
            ExtensionType fileExtension);


        /// <summary>
        /// Retrieve metadata of an object
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="fileId">File id</param>
        /// <param name="revisionId">Version number. Newest file when not set</param>
        /// <returns>Returns metadata of directory or file</returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        /// <exception cref="FileNotFoundException"></exception>
        Task<IFileMetadata> GetFileMetadata(string requestingUserId, string fileId, string revisionId);
    }
}
