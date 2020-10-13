using System;
using System.Collections.Generic;
using System.Text;

namespace LessPaper.Shared.MinIO.Interfaces
{
    public interface IMinioSettings
    {
        /// <summary>
        /// 
        /// </summary>
        string HostName { get; }


        /// <summary>
        /// 
        /// </summary>
        string AccessKey { get; }


        /// <summary>
        /// 
        /// </summary>
        string SecretKey { get;  }

    }
}
