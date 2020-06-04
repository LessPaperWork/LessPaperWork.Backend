using System;
using System.Collections.Generic;
using System.Text;
using LessPaper.Shared.MinIO.Interfaces;
using Minio;

namespace LessPaper.Shared.MinIO.Models
{
    public abstract class MinioBaseBucket
    {
        protected readonly MinioClient MinioClient;

        protected MinioBaseBucket(IMinioSettings settings)
        {
            MinioClient = new MinioClient(
                settings.Hostname,
                settings.AccessKey,
                settings.SecretKey).WithSSL();
        }

        protected MinioBaseBucket(MinioClient client)
        {
            MinioClient = client;
        }
    }
}
