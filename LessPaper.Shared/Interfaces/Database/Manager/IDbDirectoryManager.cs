using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.General;
using LessPaper.Shared.Interfaces.GuardApi.Response;

namespace LessPaper.Shared.Interfaces.Database.Manager
{
    public interface IDbDirectoryManager : IDbObjectOperations
    {
        /// <summary>
        /// Add a new directory
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="parentDirectoryId">Id of the parent directory</param>
        /// <param name="directoryName">Name of the new directory</param>
        /// <param name="newDirectoryId">Id of the new directory</param>
        /// <returns></returns>
        Task<bool> InsertDirectory(string requestingUserId, string parentDirectoryId, string directoryName, string newDirectoryId);
        
        /// <summary>
        /// Retrieve metadata of an object
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="objectId">Directory or file id</param>
        /// <param name="revisionNumber">Version number. Newest file when not set</param>
        /// <returns>Returns metadata of directory or file</returns>
        /// <exception cref="InvalidOperationException">Throws if service not available</exception>
        /// <exception cref="FileNotFoundException"></exception>
        Task<IDirectoryMetadata> GetDirectoryMetadata(string requestingUserId, string objectId, uint? revisionNumber);
    }
}
