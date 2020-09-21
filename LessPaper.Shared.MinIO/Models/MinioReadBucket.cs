using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.Bucket;
using LessPaper.Shared.MinIO.Interfaces;
using LessPaper.Shared.Models.Exceptions.Bucket;
using Minio;
using Minio.DataModel;

namespace LessPaper.Shared.MinIO.Models
{
    public class MinioReadBucket : MinioBaseBucket, IReadableBucket
    {
        public MinioReadBucket(IMinioSettings settings) : base(settings)
        {
        }

        public MinioReadBucket(MinioClient client) : base(client)
        {
        }

        public async Task Download(string bucketName, string fileId, int fileSize, Stream fileStream)
        {
                await MinioClient.GetObjectAsync(
                    bucketName, 
                    fileId, 
                    0, 
                    fileSize, 
                    async stream => await stream.CopyToAsync(fileStream));
            
        }

        public async Task DownloadDecrypted(string bucketName, string fileId, int fileSize, byte[] plaintextIvAndKey, Stream fileStream)
        {
            if (plaintextIvAndKey.Length != 16 + 32)
                throw new InvalidKeyException($"Invalid key length. Length of key was { plaintextIvAndKey.Length } bytes but must equal {16 + 32}");

            var iv = plaintextIvAndKey.Take(16).ToArray();
            var key = plaintextIvAndKey.Skip(16).ToArray();

            using var aes = Aes.Create();
            if (aes == null)
                throw new CryptoException("Unable to create aes instance");

            aes.KeySize = 256;
            using var decryptor = aes.CreateDecryptor(key, iv);
            using var cryptoStream = new CryptoStream(fileStream, decryptor, CryptoStreamMode.Write);
            var encryptedFileSize = (int)((fileSize / 16 + 1) * 16);

            await Download(bucketName, fileId, encryptedFileSize, cryptoStream);
        }

    }
}
