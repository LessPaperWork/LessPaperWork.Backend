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

    
        public async Task<bool> UploadFileEncrypted(string bucketName, string fileId, int fileSize, byte[] plaintextIvAndKey, Stream fileStream)
        {
            if (plaintextIvAndKey.Length != 16 + 32)
                return false;

            var iv = plaintextIvAndKey.Take(16).ToArray();
            var key = plaintextIvAndKey.Skip(16).ToArray();

            using var aes = Aes.Create();
            if (aes == null)
                return false;

            aes.KeySize = 256;
            using var encryptor = aes.CreateEncryptor(key, iv);
            using var cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Read);

            // ReSharper disable once RedundantCast
            var encryptedFileSize = (int)((fileSize / 16 + 1) * 16);

            return await UploadFile(bucketName, fileId, encryptedFileSize, cryptoStream);
        }

        public async Task<bool> UploadFile(string bucketName, string fileId, int fileSize, Stream fileStream)
        {
            try
            {

                await MinioClient.PutObjectAsync(
                    bucketName,
                    fileId,
                    fileStream,
                    fileSize,
                    "application/octet-stream",
                    null);

                return true;
            }
            catch (Exception e)
            {
                throw;
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> DeleteFile(string bucketName, string fileId)
        {
            try
            {

                await MinioClient.RemoveObjectAsync(bucketName, fileId);
                return true;
            }
            catch (Exception e)
            {
                throw;
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
