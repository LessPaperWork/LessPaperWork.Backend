using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.General;
using LessPaper.Shared.Interfaces.GuardApi.Response;

namespace LessPaper.Shared.Interfaces.Database.Manager
{
    public interface IDbObjectOperations
    {
        /// <summary>
        /// Get user permissions of an object (File/Directory)
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="userId">Id of the user</param>
        /// <param name="objectIds">Directory or file ids</param>
        /// <returns></returns>
        Task<IPermissionResponse[]> GetPermissions(string requestingUserId, string userId, string[] objectIds);
        
        /// <summary>
        /// Rename the directory
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="objectId">Directory or file id</param>
        /// <param name="newName">New name of the object</param>
        /// <returns></returns>
        Task<bool> Rename(string requestingUserId, string objectId, string newName);

        /// <summary>
        /// Move a file or a directory
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="objectId">Directory or file id</param>
        /// <param name="targetDirectoryId">Id of the target directory</param>
        /// <returns></returns>
        Task<bool> Move(string requestingUserId, string objectId, string targetDirectoryId);

        /// <summary>
        /// Delete a file or directory
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="objectId">Id of the object</param>
        /// <returns>Blob ids</returns>
        Task<string[]> Delete(string requestingUserId, string objectId);

        /// <summary>
        /// Prepare the sharing
        /// </summary>
        /// <param name="requestingUserId">Id of the requesting user</param>
        /// <param name="objectId">Id of the object</param>
        /// <param name="userEmails">Email addresses of the users</param>
        /// <returns></returns>
        Task<IPrepareShareData> PrepareShare(string requestingUserId, string objectId, string[] userEmails);

        /// <summary>
        /// Share a file
        /// </summary>
        /// <param name="requestingUserId"></param>
        /// <param name="shareData"></param>
        /// <returns></returns>
        Task<bool> Share(string requestingUserId, IShareData shareData);
    }

    public interface IPrepareShareFile
    {
        string FileId { get; }

        IPrepareShareRevision[] Revisions { get; set; }
    }

    public interface IPrepareShareRevision
    {
        /// <summary>
        /// Id of the revision
        /// </summary>
        string RevisionId { get; }

        Dictionary<string, IAccessKey> AccessKeys { get; }
    }

    public interface IPrepareShareData
    {
        /// <summary>
        /// Directory/File-Id which should be shared
        /// </summary>
        string RequestedObjectId { get; }

        /// <summary>
        /// Public-Keys of the requested users
        ///
        /// Key: Email
        /// Value: Public key
        /// </summary>
        Dictionary<string, string> PublicKeys { get;  }

        /// <summary>
        /// Encrypted keys of the files
        ///
        /// Key: FileId
        /// Value: Encrypted key
        /// </summary>
        IPrepareShareFile[] Files { get;  }
    }

    public interface IShareData
    {
        /// <summary>
        /// Public-Keys of the requested users
        ///
        /// Key: Email
        /// Value: With public key encrypted key
        /// </summary>
        Dictionary<string, string> EncryptedKeys { get; }

        /// <summary>
        /// Encrypted keys of the files
        ///
        /// Key: FileId
        /// Value: Encrypted key
        /// </summary>
        IPrepareShareFile[] Files { get; }
    }


}
