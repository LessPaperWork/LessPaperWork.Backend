using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LessPaper.Shared.Interfaces.Bucket;
using LessPaper.Shared.MinIO.Interfaces;
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

        public async Task<bool> DownloadFile(string bucketName, string fileId, int fileSize, Stream fileStream)
        {
            try
            {
                await MinioClient.GetObjectAsync(
                    bucketName, 
                    fileId, 
                    0, 
                    fileSize, 
                    async stream => await stream.CopyToAsync(fileStream));

                return true;
            }
            catch (Exception e)
            {
                //TODO Logging
                return false;
            }
        }

        public async Task<bool> DownloadFileDecrypted(string bucketName, string fileId, int fileSize, byte[] plaintextIvAndKey, Stream fileStream)
        {
            if (plaintextIvAndKey.Length != 16 + 32)
                return false;

            var iv = plaintextIvAndKey.Take(16).ToArray();
            var key = plaintextIvAndKey.Skip(16).ToArray();

            using var aes = Aes.Create();
            if (aes == null)
                return false;

            aes.KeySize = 256;
            using var decryptor = aes.CreateDecryptor(key, iv);
            using var cryptoStream = new CryptoStream(fileStream, decryptor, CryptoStreamMode.Write);
            var encryptedFileSize = (int)((fileSize / 16 + 1) * 16);

            return await DownloadFile(bucketName, fileId, encryptedFileSize, cryptoStream);
        }

    }
}
