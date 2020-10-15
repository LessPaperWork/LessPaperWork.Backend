using LessPaper.Shared.MinIO.Interfaces;
using Microsoft.Extensions.Options;

namespace LessPaper.ReadService.Options
{
    public class MinioSettings : IMinioSettings
    {
        public MinioSettings()
        {
            
        }

        public MinioSettings(IOptions<AppSettings> settings)
        {
            HostName = settings.Value.Minio.HostName;
            AccessKey = settings.Value.Minio.AccessKey;
            SecretKey = settings.Value.Minio.SecretKey;
            BucketName = settings.Value.Minio.BucketName;
        }


        /// <inheritdoc />
        public string HostName { get; set; }

        /// <inheritdoc />
        public string AccessKey { get; set; }

        /// <inheritdoc />
        public string SecretKey { get; set; }

        /// <summary>
        /// Bucket name
        /// </summary>
        public string BucketName { get; set; }
    }
}
