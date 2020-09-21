using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.Bucket;
using LessPaper.Shared.MinIO.Interfaces;
using LessPaper.Shared.Models.Exceptions.Bucket;
using Minio;
using Minio.DataModel;


namespace LessPaper.Shared.MinIO.Models
{
    public class MinioWriteBucket : MinioBaseBucket, IWritableBucket
    {
        public MinioWriteBucket(IMinioSettings settings) : base(settings)
        {
        }

        public MinioWriteBucket(MinioClient client) : base(client)
        {
        }


        public async Task UploadEncrypted(string bucketName, string fileId, int fileSize, byte[] plaintextIvAndKey, Stream fileStream)
        {
            if (plaintextIvAndKey.Length != 16 + 32)
                throw new InvalidKeyException($"Invalid key length. Length of key was { plaintextIvAndKey.Length } bytes but must equal {16 + 32}");

            var iv = plaintextIvAndKey.Take(16).ToArray();
            var key = plaintextIvAndKey.Skip(16).ToArray();

            using var aes = Aes.Create();
            if (aes == null)
                throw new CryptoException("Unable to create aes instance");

            aes.KeySize = 256;
            using var encryptor = aes.CreateEncryptor(key, iv);
            using var cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Read);

            // ReSharper disable once RedundantCast
            var encryptedFileSize = (int)((fileSize / 16 + 1) * 16);
            await Upload(bucketName, fileId, encryptedFileSize, cryptoStream);

        }

        public async Task Upload(string bucketName, string fileId, int fileSize, Stream fileStream)
        {
            await MinioClient.PutObjectAsync(
                bucketName,
                fileId,
                fileStream,
                fileSize,
                "application/octet-stream",
                null);

        }

        public async Task Delete(string bucketName, string fileId)
        {

            await MinioClient.RemoveObjectAsync(bucketName, fileId);

        }

        /// <inheritdoc />
        public async Task Delete(string bucketName, string[] ids)
        {
            await MinioClient.RemoveObjectAsync(bucketName, ids);
        }
    }
}
