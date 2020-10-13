using System;
using System.Collections.Generic;
using System.Text;
using LessPaper.Shared.MinIO.Interfaces;

namespace LessPaper.Shared.MinIO.Models
{
    public class MinioSettings : IMinioSettings
    {
        public MinioSettings(string hostname, string accessKey, string secretKey)
        {
            HostName = hostname;
            AccessKey = accessKey;
            SecretKey = secretKey;
        }

        public string HostName { get; }
        public string AccessKey { get; }
        public string SecretKey { get; }
    }
}
