using System;
using System.IO;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.General;

namespace LessPaper.Shared.Interfaces.ReadApi.ReadObjectApi
{
    public interface IReadObjectApi
    {
        /// <summary>
        /// Download file or directory
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="objectId">Id of file or directory</param>
        /// <param name="revisionNumber">Version number. Newest file when not set</param>
        /// <returns>Binary file</returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        /// <exception cref="FileNotFoundException"></exception>
        Task<Stream> GetObject(string requestingUserId, string objectId, uint? revisionNumber);

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
