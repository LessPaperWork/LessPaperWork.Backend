using System.IO;
using System.Threading.Tasks;

namespace LessPaper.Shared.Interfaces.Bucket
{
    public interface IReadableBucket
    {
        /// <summary>
        /// Download file from bucket
        /// </summary>
        /// <param name="bucketName">Name of the Bucket</param>
        /// <param name="fileId">Unique random file id</param>
        /// <param name="fileSize">File size in bytes</param>
        /// <param name="fileStream">Write-File-Stream</param>
        /// <returns>True if successful</returns>
        Task Download(string bucketName, string fileId, int fileSize, Stream fileStream);
        
        /// <summary>
        /// Download file from bucket and decrypt
        /// </summary>
        /// <param name="bucketName">Name of the Bucket</param>
        /// <param name="fileId">Unique random file id</param>
        /// <param name="fileSize">File size in bytes</param>
        /// <param name="plaintextKey">Plaintext decryption key</param>
        /// <param name="fileStream">Write-File-Stream</param>
        /// <returns>True if successful</returns>
        Task DownloadDecrypted(string bucketName, string fileId, int fileSize, byte[] plaintextKey, Stream fileStream);
    }
}
