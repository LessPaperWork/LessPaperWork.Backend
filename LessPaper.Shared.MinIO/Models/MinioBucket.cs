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

        public async Task<bool> UploadFileEncrypted(string bucketName, string fileId, int fileSize, byte[] plaintextKey, Stream fileStream)
        {
            return await writableBucket.UploadFileEncrypted(bucketName, fileId, fileSize, plaintextKey, fileStream);
        }
        
        public async Task<bool> UploadFile(string bucketName, string fileId, int fileSize, Stream fileStream)
        {
            return await writableBucket.UploadFile(bucketName, fileId, fileSize, fileStream);
        }

        public async Task<bool> DeleteFile(string bucketName, string fileId)
        {
            return await writableBucket.DeleteFile(bucketName, fileId);
        }

        public async Task<bool> DownloadFile(string bucketName, string fileId, int fileSize, Stream fileStream)
        {
            return await readableBucket.DownloadFile(bucketName, fileId, fileSize, fileStream);
        }

        public async Task<bool> DownloadFileDecrypted(string bucketName, string fileId, int fileSize, byte[] plaintextKey, Stream fileStream)
        {
            return await readableBucket.DownloadFileDecrypted(bucketName, fileId, fileSize, plaintextKey, fileStream);
        }
    }
}
