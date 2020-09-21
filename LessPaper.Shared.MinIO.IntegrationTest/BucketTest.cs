using System;
using System.IO;
using System.Security.Cryptography;
using LessPaper.Shared.Interfaces.Bucket;
using LessPaper.Shared.MinIO.Models;
using Microsoft.VisualStudio.TestPlatform.Common;
using Minio;
using Xunit;

namespace LessPaper.Shared.MinIO.IntegrationTest
{
    public class BucketTest
    {
        private readonly IBucket bucket;
        private const string BucketName = "lesspaper";
        private readonly byte[] file = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };


        public BucketTest()
        {
            var settings = new MinioSettings("127.0.0.1:9000", "minioadmin", "minioadmin");
            bucket = new MinioBucket(settings);
        }

        [Fact]
        public async void File_Upload_Download_Delete()
        {
            const string filename = "unencrypted_file";

            // Upload file
            await using var uploadMs = new MemoryStream(file);
            await bucket.Upload(BucketName, filename, file.Length, uploadMs);
            
            // Download file
            await using var downloadMs = new MemoryStream();
            await bucket.Download(BucketName, filename, file.Length, downloadMs);
         
            // Compare file
            Assert.Equal(file, downloadMs.ToArray());

            // Remove uploaded file
            await bucket.Delete(BucketName, filename);
        }

        [Fact]
        public async void File_Encrypted_Upload_Download_Delete()
        {
            const string filename = "encrypted_file";

            // Generate an IV (16 Byte) + Key (32 Byte)
            var ivAndKey = new byte[16 + 32];
            for (var i = 0; i < ivAndKey.Length; i++) ivAndKey[i] = (byte) i;
            
            // Upload file encrypted
            await using var uploadMs = new MemoryStream(file);
            
            await bucket.UploadEncrypted(BucketName, filename, file.Length, ivAndKey, uploadMs);
            

            // Ensure file is not readable without encryption
            await using var downloadEncryptedBlobMs = new MemoryStream(file.Length);
            await bucket.Download(BucketName, filename, file.Length, downloadEncryptedBlobMs);
            Assert.NotEqual(file, downloadEncryptedBlobMs.ToArray());

            // Download and decrypt file
            await using var downloadMs = new MemoryStream(file.Length);
            await bucket.DownloadDecrypted(BucketName, filename, file.Length, ivAndKey, downloadMs);
            Assert.Equal(file, downloadMs.ToArray());

            // Remove uploaded file
            await bucket.Delete(BucketName, filename);
        }
    }
}
