using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.Bucket;
using LessPaper.Shared.MinIO.Interfaces;
using Minio.DataModel;

namespace LessPaper.Shared.MinIO.Models
{
    public class MinioBucket : MinioBaseBucket, IBucket
    {
        private readonly IReadableBucket readableBucket;
        private readonly IWritableBucket writableBucket;

        public MinioBucket(IMinioSettings settings) : base(settings)
        {
            readableBucket = new MinioReadBucket(MinioClient);
            writableBucket = new MinioWriteBucket(MinioClient);
        }

        public async Task UploadEncrypted(string bucketName, string fileId, int fileSize, byte[] plaintextKey, Stream fileStream)
        {
            await writableBucket.UploadEncrypted(bucketName, fileId, fileSize, plaintextKey, fileStream);
        }
        
        public async Task Upload(string bucketName, string fileId, int fileSize, Stream fileStream)
        {
            await writableBucket.Upload(bucketName, fileId, fileSize, fileStream);
        }

        public async Task Delete(string bucketName, string fileId)
        {
             await writableBucket.Delete(bucketName, fileId);
        }

        /// <inheritdoc />
        public async Task Delete(string bucketName, string[] ids)
        {
            await writableBucket.Delete(bucketName, ids);
        }

        public async Task Download(string bucketName, string fileId, int fileSize, Stream fileStream)
        {
            await readableBucket.Download(bucketName, fileId, fileSize, fileStream);
        }

        public async Task DownloadDecrypted(string bucketName, string fileId, int fileSize, byte[] plaintextKey, Stream fileStream)
        {
            await readableBucket.DownloadDecrypted(bucketName, fileId, fileSize, plaintextKey, fileStream);
        }
    }
}
