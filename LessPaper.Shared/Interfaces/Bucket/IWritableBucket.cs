using System.IO;
using System.Threading.Tasks;

namespace LessPaper.Shared.Interfaces.Bucket
{
    public interface IWritableBucket
    {
        /// <summary>
        /// Encrypt and upload file
        /// </summary>
        /// <param name="bucketName">Name of the Bucket</param>
        /// <param name="fileId">Unique random file id</param>
        /// <param name="fileSize">File size in bytes</param>
        /// <param name="plaintextKey">Plaintext encryption key</param>
        /// <param name="fileStream">Read-File-Stream</param>
        /// <returns>True if successful</returns>
        Task UploadEncrypted(string bucketName, string fileId, int fileSize, byte[] plaintextKey, Stream fileStream);
        
        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="bucketName">Name of the Bucket</param>
        /// <param name="fileId">Unique random file id</param>
        /// <param name="fileSize">File size in bytes</param>
        /// <param name="fileStream">Read-File-Stream</param>
        /// <returns>True if successful</returns>
        Task Upload(string bucketName, string fileId, int fileSize, Stream fileStream);

        /// <summary>
        /// Delete file from bucket
        /// </summary>
        /// <param name="bucketName">Name of the Bucket</param>
        /// <param name="fileId">Unique random file id</param>
        /// <returns></returns>
        Task Delete(string bucketName, string fileId);


        /// <summary>
        /// Delete many files from bucket
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="ids">Ids of the files</param>
        /// <returns></returns>
        Task Delete(string bucketName, string[] ids);
    }
}
