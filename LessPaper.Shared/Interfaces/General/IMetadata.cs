using System;
using System.Collections.Generic;
using LessPaper.Shared.Enums;

namespace LessPaper.Shared.Interfaces.General
{
    public interface IMetadata : IIdentifiable
    {
        /// <summary>
        /// Filename
        /// </summary>
        string ObjectName { get; }
        
        /// <summary>
        /// Permissions
        /// 
        /// Key: UserId
        /// Value: Permission (Flags)
        /// </summary>
        Dictionary<string, Permission> Permissions { get; }

        /// <summary>
        /// Path
        /// </summary>
        string Path { get;  }
    }
}
